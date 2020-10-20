# Parameterization templates

## Description

Parameterization or customization templates allow to quantify the interaction set permutations and KMC attempt frequencies that were found for a correctly defined model. They can only be created if a model passes validation and should usually be recreated once the underlying model changes.

### [**Known issues**](#known-issues)

1. It is currently not possible to set the local site energies by a customization template. Instead, these values have to be defined in the [energy model](./energy-model.md), but they can be replaced without invalidating the parameterization templates affiliated with the model.
2. The so-called "hidden transition rules", that is, symmetry dependent to the displayed one, of KMC transitions cannot be viewed in the GUI, only the number of hidden rules is visible. In the majority of cases, these rules are simply the inversion of the original rule.

## Template objects

### [Transition rule sets](#transition-rule-sets)

One transition rule set is created for each defined KMC transition of the underlying model. These sets contain the symmetry reduced collection of state change rules for a transitions and are used to quantify the attempt frequency of each event in the context of the affiliated transition. Each transition rule defines the three states $S_0,S_1,S_2$ as explained in the [transition model section](./transition-model.md). Each rule has at least one hidden rule which describes its symmetry dependent inverted execution. For example, for a basic vacancy migration with species $A$ and vacancy $V_A$, only one of the state triplets shown in (1) will be listed in the parameterization template.

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
\tag{1}
$$

### [Pair interaction sets](#stable-pair-interaction-sets)

One pair interaction set is created for each symmetry reduced stable and unstable pair interaction that was found by Mocassin based on the constraints defined in the [energy model](./energy-model.md). Each set contains the symmetry reduced set of permutations based on the possible occupations of the two involved unit cell sites, a reference geometry that describes the interaction, and a distance information. It further provides information on how many symmetry equivalent interactions of this type exist on the two involved cell sites after $P1$ extension of the data. 

For stable pair interactions, it is generally recommended to model only defect interactions and leave permutations that contain one or two default occupations at 0 eV. For example, the four permutations of an oxygen site to cerium site interaction in reduced ceria could be modelled as shown in (2).

$$
\begin{aligned}
    E(\mathrm{O_O^x-Ce_{Ce}^x}) = 0~\text{eV} \\
    E(\mathrm{O_O^x-Ce_{Ce}'}) = 0~\text{eV}  \\
    E(\mathrm{v_O^{\bullet\bullet}-Ce_{Ce}^x}) = 0~\text{eV}  \\
    E(\mathrm{v_O^{\bullet\bullet}-Ce_{Ce}'}) \neq 0~\text{eV}
\end{aligned}
\tag{2}
$$

Consult the [energy model page](./energy-model.md) for information on how the energy values of stable and unstable pair interactions are used to calculate the three state energies $E(S_0),E(S_1),E(S_2)$ of transition attempts during a simulation.

One potentially confusing information is that two pair interaction sets may be chiral to each other. Creating a $P1$ symmetry extension of an $A-A'$ interaction does not always contain the geometric inversion $A'-A$ as the interaction contains direction information. The extension create two vector fields, which can be chiral to each other and thus a pair interaction can show chirality. However, as it is impossible to calculate two distinct energy values for the two interactions, the system automatically links the two sets and does not allow independent modelling of the permutations.

### [Group interaction sets](#group-interaction-sets)

One group interaction set contains all symmetry reduced permutations of its affiliated user-defined cluster geometry. It further contains an information how many unique geometries per center site exist after the $P1$ symmetry extension. Importantly, the permutations are given as a linear chain of particles with the first particle sitting at the center site and the others on the surrounding geometry in order of definition. Taking the example cluster of reduced ceria as described in the [energy model section](./energy-model.md), the group interaction set contains three entries as shown in (3), where $\mathrm{O_u}$ is an oxygen on the unstable transition site, and has one unique geometry per center site. The missing fourth permutation is symmetry equivalent to the second one and cannot be modelled independently.

$$
\begin{aligned}
    E(\mathrm{O_u-Ce_{Ce}^x-Ce_{Ce}^x}) = 0~\text{eV} \\
    E(\mathrm{O_u-Ce_{Ce}^x-Ce_{Ce}'}) = 0~\text{eV}  \\
    E(\mathrm{O_u-Ce_{Ce}'-Ce_{Ce}'}) = 0~\text{eV}
\end{aligned}
\tag{3}
$$

