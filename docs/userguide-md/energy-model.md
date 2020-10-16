# The energy model

## Description

The energy model defines the constraints the system uses to search, process, and permute interactions for the model. The system supports pair interactions, multi-body interactions with up-to 9 members, and local site defect energies. Contrary to its name, the energy model only defines energy values for the local site energy directly, while the interactions are quantified later, after symmetry processing and permutation of the possible occupations.

### [**Known issues**](#known-issues)

- The system enforces that a permutation $A-B$ of a pair interaction between two stable sites is automatically symmetry equivalent to its inversion $B-A$. This causes an issue for pair interactions between two distinct sublattices $L_1,L_2$ if one of the particles $A,B$ exists in both sublattices. In this case, $A-B=B-A$ is not necessarily true but the system does not allow to set two different energy values.
- The solver outputs the total lattice energy of the system as half of the sum of the energy contributions of all positions. This method yields a valid energy only if the entire energy landscape is modelled solely with pair interactions

## The energy model objects

### [Basics](#basics)

A common way to model complex interactions in a fixed position system is based on a summation of multi-body interactions of increasing size till a certain cutoff. In this fashion, each position of the system can be assigned an energy contribution $E_i$ that depends on the occupation state of the position and the surrounding positions. The first two contributions in this summation is the local site energy $\epsilon_i$ (self interaction) and the set of pair interaction energies $\epsilon_{ij}$, that is, the center position $i$ forms a two body cluster with each of the surrounding positions $j$ within the cutoff range. Due to symmetry, each pair usually has an $N_{ij}\in\N$ prefactor that is not unity. Then, more complex multi-body interactions can be added by creating all possible permutations with up to $n$ members as shown in (1).

$$
E_i=\epsilon_i + \sum_j N_{ij} \cdot \epsilon_{ij} + \sum_j\sum_k N_{ijk} \cdot \epsilon_{ijk} +...+\sum_j...\sum_n N_{ij...n} \cdot \epsilon_{ij...n}
\tag{1}
$$

Obviously, this quickly results in a huge number of permutations which are no longer calculable by means of DFT or force field methods. Thus, Mocassin cuts the summation after the second term and instead allows the user to create custom multi-body interactions with up-to 9 members that provide an energy contribution $\epsilon_{ik}$ to finetune the default pair interaction based model approach as shown in (2).

$$
E_i=\epsilon_i + \sum_j N_{ij} \cdot \epsilon_{ij} + \sum_k N_{ik} \cdot \epsilon_{ik} 
\tag{2}
$$

This approach has the advantage that the symmetry reduced set of local site energies and pair interactions, as well as the symmetry-reduced permutation set of the user-defined clusters, can be created automatically by the system with very little effort. This yields parameterization templates that are used to quantify all permutations and interactions and which can be exchange without changes to the underlying energy model definition.

The energies of the two stable [states](./transition-model.md) $E_0,E_2$ of the system during a transition are the simply described according to (3). Here $i$ refers only to the position belonging to the transition path, as the rest of the contributions to the system energy do not change during the transition.

$$
\begin{aligned}
   E_0=\sum_i E_i(S_0) && E_2=\sum_i E_i(S_2)
\end{aligned}
\tag{3}
$$

Since allowing exact descriptions for the transition state energies $E_1$ introduces potential, hard to validate consistency issues, the system uses a model description for the energy of $S_1$ as shown in (4). The first term on the righthand side is a linear interpolation between the two affiliated stable states. The second term is the summation of all defined *unstable-stable* interactions during $S_1$, that is, they form an additive contribution. The last term describes the change in potential energy $\Delta E_f$ due to the displacement of charged particles between $S_0$ and $S_2$ in an applied external electric field $\vec{E}_f$, of which half affects $S_1$ on average. The term can be derived from the shift of the focal point $\Delta \vec{x}_s$ of the total charge $q_\mathrm{tot}$ of the transition path by solving (5) for a set of points charges.

$$
E_1=\frac{E_2-E_0}{2}+\sum_i E_i(S_1)+\frac{1}{2} \cdot \vec{E}_f \cdot \sum_i (\vec{x}_{i,2}-\vec{x}_{i,0})\cdot q_i
\tag{4}
$$
$$
\Delta E_f = \frac{1}{2} \cdot \Delta \vec{x}_s \cdot q_\mathrm{tot} \cdot \vec{E}_f
= \frac{1}{2} \cdot \vec{E}_f \cdot \left(\frac{\int \vec{x}_2~\mathrm{d}q}{\int \mathrm{d}q}-\frac{\int \vec{x}_0~\mathrm{d}q}{\int \mathrm{d}q}\right)\cdot \int \mathrm{d}q
\tag{5}
$$

To provide the required data for the parameterization template creation process, the energy model provides the following model objects:

1. **Stable environment**
   - What are the cutoff and filter settings for sampling the environment of stable positions?
2. **Unstable environment**
   - What are the cutoff and filter settings for sampling the environment of each unstable position?
3. **Interaction cluster** *(group interaction)*
   - What are the reference geometries that form multi-body interactions?
4. **Site energy**
   - What is the offset energy of each particle on each of its supported positions?

### [Stable environment](#stable-environment-cutoff-range)

The stable environment object defines the limitations for interaction definition around all stable positions. This is done using a cutoff radius, which limits the maximum interaction range, and a set of hollow sphere filters that allow to filter interactions between specific sublattices within a specified radial range. The properties defined by the filters are: (i) the start and end radius between which the filter is applied; and (ii) the two sublattices or reference sites $L_1,L_2$ for which the interactions are removed.

From this data, the system automatically creates a list off all existing pair interactions. It is important to note, that each multi-body interaction is later encoded for the simulations with the help of the pair interaction geometric information, that is, filtering these pair interactions causes a validation error in the system. It is not possible to define a distinct environment for each stable site separately. This is due to consistency reasons to ensure that no energy can be gained or lost by a simple change of the reference point. This also implies that interaction filters always affect both affiliated sites.

### [Unstable environments](#unstable-environments)

Unstable environments provide the same model information as the stable environment. However, it is possible to define one environment per unstable site as *unstable-stable* interactions exist only as a purely additive component to the [transition state energy](./transition-model.md) $E_1$ as shown in (4).

### [Interaction clusters](#group-interaction)

Interaction clusters describe complex multi-body interactions with an origin point, also called center, and up-to eight "surrounding" positions. The origin point must be any of the reference sites added to the [structure model](./structure-model.md) and the surrounding positions are defined as a list of absolute $(a,b,c)$ vectors. The geometry of a cluster is symmetry extended and permuted as an entity. Thus, cluster occupation permutations that can be mapped onto each other using the symmetry operations of the selected space group are treated as equivalent, that is, they cannot have distinct energy contributions.

It is important to note that clusters only affect the energy of the site that is defined as the origin point. To get the same cluster from any of the surrounding positions, it is required to define the cluster with that position as the new origin site.

### [Site energies](#site-energies)

Site energies allow to offset the energy of a positions based on the occupation, by default all site energies are set to 0 eV. The most basic usage is to define a fixed migration barrier by setting a mobile species on an unstable site to the barrier energy.

## Model examples

### [Ceria](#ceria)

To define a very simple energy landscape for oxygen migration KMC in ceria, the following steps are required:

1. Define a non-zero interaction range for the stable environment that is at least the closest distance between a cerium and an oxygen position
2. An example for a valid cluster uses the previously defined unstable oxygen migration site as the origin point with $(0.5, 0.5, 0)$ and $(0.5, 0, 0.5)$ as the surrounding positions. This gives the "migration edge" as desribed in the [original publication](http://dx.doi.org/10.1002/jcc.26418).