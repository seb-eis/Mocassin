# The transition model

## Description

The transition model controls the list of symmetry-reduced KMC and MMC reference events that are defined for a model. These transitions describe the possible system change events and where they can occur in the geometry. In contrast to other model objects, transitions form a pool of options for later binding to simulation definitions, that is, defined objects are not automatically active in a simulation. The system supports a variety of transport mechanisms including complex cases such as interstitialcy and vehicle movement.

## The transition model objects

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

Here, $k_\mathrm{B}$ is the Boltzmann constant and $T$ is the thermodynamic temperature. Depending on the simulation type, there are special cases that are not simply described by a success/failure chance $P$ as explained in on the [simulator page](./the-simulator.md). The description of transitions in Mocassin by means of the states $E_0,E_1,E_2$ is abstracted into four components to make full use of the symmetry of the system:

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

A state change chain or 'abstract transition' is a linearized, geometry-independent abstraction of an MMC or KMC transition event that defines a sequence of state change groups with linkers. The only geometric information it contains is how many positions $p_0,...,p_n$ are part of the transition process and in which order the state change groups appear, no 3D geometry is specified. Two types of linking between two consecutive state change groups exist: 'dynamic' ($\rightarrow$) and 'static' ($\bullet$) which denote dependent or non-dependent occurrence of state changes. The processing system automatically infers possible state changes rules - known as 'transition rules' - from these chains definitions, which contain the path occupations during $S_0,S_1,S_2$ as previously shown in the matrix notations (3) and (4), by applying several boundary conditions such as mass conservation and charge conservation.

For practical reasons, there is no need to manually define the linker/connection pattern . The system is rather abstract and there is a limited number of meaningful transitions that can be described with the current linkers and limit of 8 positions per transition. While not all theoretically possible cases are available, the most important mechanisms can be selected:

- 2-Site Metropolis
  - Description: A regular MMC exchange between two stable sites
  - Site chain : *stable, stable*
  - Link chain : $p_0 \rightarrow p_1$
- 3-Site Migration
  - Description: A simple KMC migration where $p_1$ is the unstable transition sites of the migrating species
  - Position chain : *stable, unstable, stable*
  - Linking chain : $p_0 \rightarrow p_1 \rightarrow p_2$
- 5-Site Interstitialcy
  - Description: An interstitialcy mechanism where one species pushes another and $p_1,p_3$ are the unstable transition sites
  - Position chain : *stable, unstable, stable, unstable, stable*
  - Linking chain : $p_0 \rightarrow p_1 \rightarrow p_2 \rightarrow p_3 \rightarrow p_4$
- 7-Site Interstitialcy
  - Description: An interstitialcy mechanism where one species pushes two other species and $p_1,p_3,p_5$ are the unstable transition sites
  - Position chain :*stable, unstable, stable, unstable, stable, unstable, stable*
  - Linking chain : $p_0 \rightarrow p_1 \rightarrow p_2 \rightarrow p_3 \rightarrow p_4 \rightarrow p_5 \rightarrow p_6$
- 5-Site 2-Species Vehicle
  - Description: A transport of two parallel 3-site migrations where both migrating species $A,B$ are modelled to share a common unstable transition site with a custom species that represents the $AB$ pseudoparticle at $p_2$
  - Position chain : *stable, stable, unstable, stable, stable*
  - Linking chain : $p_0 \rightarrow p_1 \bullet p_2 \bullet p_3 \rightarrow p_4$
- 6-Site 2-Species Vehicle
  - Description: A transport of two parallel 3-site migrations where both migrating species $A,B$ are modelled to have unstable transition sites $p_1,p_3$
  - Position chain : *stable, unstable, stable, stable, unstable, stable*
  - Linking chain : $p_0 \rightarrow p_1 \rightarrow p_2 \bullet p_3 \rightarrow p_4 \rightarrow p_5$

Aside from the position chain and the selection of a linking pattern, state change chains also allow to set an "association/dissociation" flag. This flag has an effect only if it is used with vehicle connection patterns and instructs the processing system to generate association/dissociation transition rules based on the linear chain instead of the default unidirectional movement behavior. As an example, using a 5-site 2-species vehicle with a parallel $A$ and $B$ migration by vacancy mechanism, the matrix notations in (6) and (7) illustrate the different results for unidirectional (default) transport and association/dissociation behavior, respectively. It should be obvious that the two cases are different processes with distinct net absolute charge transports in the general case when applied to a 3D geometry, thus Mocassin allows to define and model them independently.

$$
\begin{aligned}
    \begin{bmatrix}
        & p_0 & p_1 & p_2 & p_3 & p_4 \\
        S_0 & A & V_A & \empty & B & V_B \\
        S_1 & \empty & \empty & AB & \empty & \empty \\
        S_2 & V_A & A & \empty & V_B & B
    \end{bmatrix}
    \begin{bmatrix}
        & p_0 & p_1 & p_2 & p_3 & p_4 \\
        S_0 & V_A & A & \empty & V_B & B \\
        S_1 & \empty & \empty & AB & \empty & \empty \\
        S_2 & A & V_A & \empty & B & V_B
    \end{bmatrix}
\end{aligned}
\tag{6}
$$

$$
\begin{aligned}
    \begin{bmatrix}
        & p_0 & p_1 & p_2 & p_3 & p_4 \\
        S_0 & A & V_A & \empty & V_B & B \\
        S_1 & \empty & \empty & AB & \empty & \empty \\
        S_2 & V_A & A & \empty & B & V_B
    \end{bmatrix}
    \begin{bmatrix}
        & p_0 & p_1 & p_2 & p_3 & p_4 \\
        S_0 & V_A & A & \empty & B & V_B \\
        S_1 & \empty & \empty & AB & \empty & \empty \\
        S_2 & A & V_A & \empty & V_B & B
    \end{bmatrix}
\end{aligned}
\tag{7}
$$

### [MMC and KMC transitions](#mmc-and-kmc-transitions)

The KMC and MMC transition objects are the last step in the definition of state change events for the model. They bind the previously defined state change chains to an arbitrary number of 3D reference geometries to instruct the model processing system where these changes can occur in the lattice. Analogously to sites, these geometric definitions are processed by using the space group symmetry. This means that the effort to fully define the KMC or MMC transition set of the unit cell is usually minimal.

For MMC transitions it is sufficient to specify the state change chain, which can only be of the 2-site Metropolis type, and select the unit cell reference sites for $p_0$ and $p_1$. All existing permutations of affiliated position pairs in the unit cell can be inferred from this definition and the generated set of transition rules can be reduced to the set that is actually applicable based on the possible occupations of the selected reference sites.

For KMC transitions it is sufficient to specify the state change chain, which cannot be of the 2-site Metropolis type, and define a chain of 3D positions that describes one reference path where the state change can occur. Again, all existing unique paths of the unit cell and the possible set of transition rules can be deduced from this information based on symmetry and occupation limitations. For example, assuming space group $\mathrm{P \text{-} 1}$ which has the operations $(x,y,z)$ and $(-x,-y,-z)$, all 3D path definitions in (8), with 
$(a,b,c)$ vectors in unit cell coordinates, are symmetry equivalent. Thus defining either one with a $(\pm n_a,\pm n_b,\pm n_c)$ $n_i \in \N$ translation transformation implies existence of the others in Mocassin.

$$
\begin{aligned}
(0,0,0) \rightarrow (+1,0,0) \rightarrow (+2,0,0) \\
(0,0,0) \rightarrow (-1,0,0) \rightarrow (-2,0,0) \\
\end{aligned}
\tag{8}
$$

## Model examples

### [2-Site MMC](#2-site-ionic-mmc)

To define an MMC exchange of two species $A,B$ between two sites, the following steps are required:

1. Define a $[A,B]$ state change object
2. Define a $\{[A,B]\}$ state change group
3. Define a state change chain with the '2-Site Metropolis' connection pattern and set the state change group list to $\{[A,B]\},\{[A,B]\}$
4. Define a Metropolis transition using the created chain and select the two reference sites between which the exchange should occur. Selecting the same site twice causes the exchange to take place within one sublattice

### [3-Site vacancy KMC](#3-site-vacancy-kmc)

To define a KMC vacancy migration of a species $A$ between two sites, the following steps are required:

1. Define a $[A,V_A]$ and a $[A,\empty]$ state change object
2. Define the $\{[A,B]\}$ and $\{[A,\empty]\}$ state change groups
3. Define a state change chain with the '3-Site Migration' connection pattern and set the state change group list to $\{[A,B]\},\{[A,\empty]\},\{[A,B]\}$
4. Define a Kinetic transition using the created chain and define a reference path for the migration where the position stability of targeted positions is *stable, unstable, stable*

### [5-Site vehicle KMC](#5-site-vehicle-kmc)

To define a parallel KMC vehicle migration of two species $A$ and $B$ with a shared unstable transition position that will be occupied by a combined pseudoparticle $C=AB$, the following steps are required:

1. Define a $[A,V_A],[B,V_A],[C,\empty]$ state change objects
2. Define a $\{[A,V_A]\},\{[B,V_A]\},\{[C,\empty]\}$ state change objects
3. Define a state change chain with the '5-Site 2-Species Vehicle' connection pattern and set the state change group list to $\{[A,V_A]\},\{[A,V_A]\},\{[C,\empty]\},\{[B,V_A]\},\{[B,V_A]\}$
4. (Optional) Activate the 'association/dissociation' flag if this behavior is required
5. Define a Kinetic transition using the created chain and define a reference path for the migration where the position stability of targeted positions is *stable, stable, unstable, stable, stable*

**Warning:** It should be kept in mind that 'association/dissociation' is defined in the context of the abstract, linearized state change chain object rather than the 3D binding geometry. That is, depending on how the order of the binding positions is chosen, the behavior in 3D space can be inverted as shown in (9).

$$
\begin{aligned}
    \text{unidirectional}&: p_0 \rightarrow p_1 \bullet p_2 \bullet p_3 \rightarrow p_4 \\
    \text{asso-/dissociation}&: p_1 \rightarrow p_0 \bullet p_2 \bullet p_3 \rightarrow p_4
\end{aligned}
\tag{9}
$$

### [Charge transport](#charge-transport)

Defining a charge transport or small-polar hopping is done analogously to ionic vacancy migration. However, the species $A,B$ must fulfill a set of requirements for the model builder to correctly recognize and process a charge transport, namely:

- $A$ and $B$ must have a charge differnce $\Delta q \ne 0$
- $A$ and $B$ must have different element symbols if they are not the same species or the mass conservation rules will not be applied and rule generation will create physically invalid rules

It is further highly recommended to follow some guidelines in order to allow the solver system to directly output correct conductivity and mobility values in the text dump. Otherwise, these quantities will be calculated using the wrong charge values as in contrast to the model builder, the solver system does not differ between a physical ion migration and a charge transport.

- Define a special pseudoparticle $C$ that has the charge value $\Delta q$ to describe the occupation of the unstable position during $S_1$
- Define relative charges for your 'donor' and 'acceptor' particles, that is, for modelling a polaron hopping between $\mathrm{Ce}^{3+}$ and $\mathrm{Ce}^{4+}$ in ceria the particles $\mathrm{Ce^0,Ce^{-1}}$ should be defined similar to the Kröger-Vink notation $\mathrm{Ce^x,Ce'}$.

**Note:** Not all charge transport processes can be correctly described using the Boltzmann probability function (2). Mocassin does not support custom or distinct probability function definition for each transition out of the box, it can however be achieved with minor adjustments to the solver source code.