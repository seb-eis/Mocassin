# Transition model

## The MC states and transition model objects
---

### [Basics](#basics)

Monte Carlo transitions in Mocassin are based on transition state theory, that is, the system has three occurring states during a transition attempt: initial state $S_0$, transition state $S_1$, and final state $S_2$, where the $S_1$ state is omitted for MMC simulations. Each of the states has an energy value $E_0,E_1,E_2$ that is calculated for each transition attempt and the probability of transition success is described by Boltzmann statistics (1) and (2):

$$
\begin{aligned}
P_{KMC}&=\mathrm{exp}(-\frac{E_1-E_0}{k_\mathrm{B}\cdot T})
\tag{1}
\end{aligned}
$$
$$
\begin{aligned}
P_{KMC}&=\mathrm{exp}(-\frac{E_1-E_0}{k_\mathrm{B}\cdot T})
\tag{2}
\end{aligned}
$$

Here, $k_\mathrm{B}$ is the Boltzmann constant and $T$ is the thermodynamic temperature. Depending on the simulation type, there are special cases that are not simply described by a success/failure chance $P$ as explained in the [MMC](#the-mmc-routine) and [KMC](#the-kmc-routine) sections. The description of transitions in Mocassin by means of the states $E_0,E_1,E_2$ is abstracted into four components to make full use of the symmetry of the system:

1. **State change** (*State exchange pair*)
   - How does the state of a single involved position change from one particle to another?
2. **State change group** (*State exchange group*)
   - How can similar state exchanges be grouped together?
3. **State change chain** (*Abstract transition*)
   - How can state change group groups form a linear chain of of changes?
   - What are the interdependencies between adjacent groups?
4. **KMC/MMC Transition** (*Metropolis/Kinetic Transition*)
   - Where can an abstract transition happen in the system according to a reference geometric path?

### [State changes](#state-changes)
A state change or 'state exchange pair' describes two possible states of a position as two particles that can be interchanged during a transition. They are noted as $[D,A]$ in this manual and the original [publication](https://doi.org/10.1002/jcc.26418), where $D$ is the 'donor' state/particle and $A$ the 'acceptor' state/particle. This was chosen for illustrative purposes only with the 'donor, acceptor' formalism from chemistry in mind. The Mocassin transition processing system itself has no means of processing if a custom particle is a 'donor' or 'acceptor', it only enforce that the non-interacting void/null state $\empty$ as defined in the [particle model](./particle-model.md) is always an acceptor state.

A simple example for is an MMC exchange within a sublattice which can be occupied by two particle types $A,B$. During an MMC transition, which is a particle/state swap between two positions, there are two exchanges possible that change the system: $A \rightarrow B$ and $B \rightarrow A$. Both changes must happen at different locations in the lattice to produce a change in the system state. Thus, the local state change is valid for both involved positions $p_0,p_1$. A matrix notation to describe states over positions illustrates that two options exist:

$$
\begin{aligned}
    \begin{bmatrix}
        & p_0 & p_1 \\
        S_0 & A & B \\
        S_2 & B & A
    \end{bmatrix}
    \begin{bmatrix}
        & p_0 & p_1 \\
        S_0 & B & A \\
        S_2 & A & B
    \end{bmatrix}
\end{aligned}
\tag{3}
$$

The matrix representations can be trivially converted to each other by inverting the order of the states, that is, they describe the same process. This is hardly surprising for symmetry reasons; if one event is the forward execution, the backwards execution must be analogously possible or the system would contain an irreversible process. Thus, the only state change required for this MMC process is $[A,B]$.

In the case of an KMC event, there are at least three positions $p_0,p_1,p_2$ involved, where $p_0,p_2$ are stable and occupied during $S_0,S_2$ and $p_1$ is unstable and only occupied during $S_1$. Assuming a simple case of a vacancy migration with three positions, mobile species $A$ (donor), and vacancy $V_A$ (acceptor), the position occupations during the three states can be written in matrix  notation as shown in (4). Here, an occupation by the null/void particle $\empty$ denotes that a position is in an non-interacting state.

$$
\begin{aligned}
    \begin{bmatrix}
        & p_0 & p_1 & p_2 \\
        S_0 & A & \empty & V_A \\
        S_1 & \empty & A & \empty \\
        S_2 & V_A & \empty & A
    \end{bmatrix}
    \begin{bmatrix}
        & p_0 & p_1 & p_2 \\
        S_0 & V_A & \empty & A \\
        S_1 & \empty & A & \empty \\
        S_2 & A & \empty & V_A
    \end{bmatrix}
\end{aligned}
\tag{4}
$$

Before we can define the required $[D,A]$ pairs, it is important to understand that Mocassin enforces two implicit behaviors during transition processing that are a direct result of the consistency rules of the [energy model](./energy-model.md) principle:

1. Unstable positions cannot create an energy contribution to $E_0,E_2$, that is, they are forced to be in the $\empty$ state during $S_0,S_2$
2. Stable positions cannot create a direct energy contribution to $E_1$, that is, they are forced to be in the $\empty$ state during $S_1$.

This has two consequences for the state change definition process: (i) it is never required to define an $[D,\empty]$ for a stable position, the system deduces and enforces them for any $S_{0,2}\rightarrow S_1$ automatically; and (ii) any $[D,A]$ for unstable positions must be an $[D,\empty]$ pair.

Keeping theses rules in mind and advancing from $S_0 \rightarrow S_2$, the positions $p_0,p_2$ perform the local states changes $A \rightarrow V_A$ and $A \rightarrow V_A$, or vise-versa, respectively. This means that the first required local state change is $[A,V_A]$. The second pair must describe what to place on the unstable position $p_1$ which gives $[A,\empty]$ as the second state change.

### [State change groups](#state-change-groups)

A state change group or 'state exchange group' describes a set of state changes $\{[D_0,A_0],...,[D_n,A_n]\}$ that share analogous properties. It is a convenience object that allows to group multiple mobile species into the same transition. In the majority of cases each group contains only one state change object. A typical example for more than one could be to describe all halide ions to move in the same vacancy fashion by defining the state exchange group for the stable positions $p_0,p_2$ as shown in (5).

$$
\{[\mathrm{F}^-,V],[\mathrm{Cl}^-,V],[\mathrm{Br}^-,V][\mathrm{I}^-,V]\}\tag{5}
$$

It should be noted that defining and using them as an all-in-one state change group instead of defining individual transitions for each element has an effect on how the simulation movement tracking system will behave as explained in the [movement tracking](./movement-tracking.md) section.

### [State change chains](#state-change-chains)

A state change chain or 'abstract transition' is a linearized, geometry-independent abstraction of an MMC or KMC transition event that defines a sequence of state change groups with linkers. The only geometric information it contains is how many positions $p_0,...,p_n$ are part of the transition process and in which order the state change groups appear, no 3D geometry is specified. Two types of linking between two consecutive state change groups exist: 'dynamic' and 'static' which denote dependent or non-dependent occurrence of state changes. The processing system automatically infers possible state changes rules - known as 'transition rules' - from these chains definitions, which contain the transition path occupation during $S_0,S_1,S_2$ as previously shown in the matrix notations (3) and (4).

## [The MMC Routine](#the-mmc-routine)
---
## [The KMC Routine](#the-mmc-routine)
---