# The particle model

## Description

The particle model is the entry point to any Monte Carlo model in Mocassin and probably the most intuitive component. It specifies which species exist within the simulation and in which occupation sets the specified species occur. Due to the way the system packages data for simulation, the number of custom species is limited to 63 of which all can be mobile. It is generally recommended to define only data that is required in the model as additional entries usually bloat the model with dead permutations and dead species indices.

## The particle model objects

### [Basics](#basics)

Mocassin defines two model objects to describe the current state or occupation of a position and the list of possible states or occupations of a position.

1. **Particle**
   - What is the state or occupation of a position?
2. **Particle set**
   - What are the option sets that exist for permutation generation?

### [Particles](#particles)

Particles describe a state that can be assigned to a position during model building and simulation. In the majority of cases, the name 'particle' is accurate as it describes an ionic species, e.g. an oxygen ion $\mathrm{O^{2-}}$. However, they can also describe pseudoparticle states, such as vacancies, model different charge states of the same species, or define absence of anything. The latter exists as the implicit void or null particle $\empty$ and is used by the system to model a non-interacting state. Custom particle objects require three properties for a full definition.

The *symbol* property of the particle assigns an arbitrary tag which is used internally to determine how mass conservation rules must be respected during processing. For example, by defining that two species $A'$ and $A''$ share the same *symbol* $A$, the system is told that interchanging them does not violate mass conservation. The *charge* property assigns the charge state $q$ as a multiplicator $z \in \R$ for the elemental charge $e$, evaluated as $q=z \cdot e$, and is used for electric field influence calculation during KMC, charge conservation, and detection of charge transport mechanisms in KMC. The last property is the *vacancy flag* which marks a particle to behave like a vacancy pseudoparticle. Vacancies are always required to model physical movement in Mocassin and they are also used to model empty states in an interstitial mechanism. It is allowed and recommended to define multiple vacancies for multiple mechanisms or sites to allow the solver [tracking systems](movement-tracking.md) to trace them individually during simulation. 

**Important:** It is required to set the either the charge of vacancies to $z=0$ or of their affiliated default occupation. This may be counter intuitive at first but if $z(A)=-z(V_A)$ then the effective charge transport of a vacancy migration would be zero.

### [Particle sets](#particle-sets)

Particle sets are basically groups of allowed states that can be assigned to unit cell sites in the [structure model](./structure-model.md) to control the permutation options of that site. They are a simple set of particles $\{A_0,...,A_n\}$ without further properties. Every set implicitly contains the $\empty$ state which will be automatically used by the model processor where required.

The first particle of a particle set is the default state of that set if applied to a stable site, that is, the site will be occupied by this state before the doping logic is applied to simulation supercells. For sets applied to unstable sites, the default state is always $\empty$ as unstable positions cannot be in an interacting state when the lattice is in a stable state.

## Model examples

### [Reduced ceria](#reduced-ceria)

The following model definitions are required to define reduced ceria $\mathrm{CeO_2}$ and later allow both oxygen migration and polaron hopping:
1. A $\mathrm{Ce^{4+}}$ particle, e.g. *symbol* = "Ce", $z=+4$
2. A $\mathrm{Ce^{3+}}$ particle, e.g. *symbol* = "Ce", $z=+3$
3. A $\mathrm{O^{2-}}$ particle, e.g *symbol* = "O", $z=-2$
4. A $\mathrm{V_O}$ particle, e.g. *symbol* = "Vo", $z=0$
5. A $\mathrm{e^-}$ particle, e.g. *symbol* = "e", $z=-1$
6. A $\{\mathrm{O^{2-},V_O}\}$ particle set for the oxygen site
7. A $\{\mathrm{Ce^{4+},Ce^{3+}}\}$ particle set for the cerium site
8. A $\{\mathrm{O^{2-}}\}$ particle set for the unstable oxygen transition site
9. A $\{\mathrm{e^{-}}\}$ particle set for the unstable polaron transition site