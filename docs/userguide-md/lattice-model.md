# The lattice model

## Description

The lattice model describes how the system should create supercells for simulations. It defines the default occupation of the unit cell, which dopings can be quantified later, and in which order the dopings are applied during the supercell generation process.

### [**Known limitations**](#known-limitations)

The lattice simulation generation system is very fast but also very limited. It can only create doped supercells based on a single default occupation unit cell and uniform random placement of dopants. Custom cells with different doping zones/blocks or concentration gradients are not supported and have to be created by external programs and then must be injected into the simulation databases after the build process or a lattice must from a previous simulation with compatible lattice settings must be imported. The reason is that in the majority of cases the placement of static defects within a simulation should either be done in a uniform random fashion or be created by an MMC simulation from which the lattice is recycle for a KMC simulation.

## Lattice model objects

### [Basics](#basics)

The most basic doping of a crystalline material one can imagine is a simple replacement of one ion by another. An example would be the replacement of the tetravalent ion $\mathrm{B(IV)}$ of an $\mathrm{A(II)B(IV)O_3}$ perovskite by another tetravalent ion $\mathrm{C(IV)}$ as shown in the formal reaction (1).

$$
\mathrm{A(II)B(IV)O_3} + \mathrm{C(IV)O_2} \rightarrow \mathrm{A(II)C(IV)O_3} + \mathrm{B(IV)O_2}
\tag{1}
$$

Doping a crystalline material usually has sides effects that introduce additional defects aside the main impurity into the system. For eample, assuming that the doping introduces a trivalent ion $\mathrm{C(III)}$ on the $\mathrm{B}$ site of the perovskite, then this usually results in oxygen vacancies $\mathrm{v_O^{\bullet \bullet}}$ to compensate for the charge difference as shown in (2) using [Kröger-Vink notation](https://en.wikipedia.org/wiki/Kr%C3%B6ger%E2%80%93Vink_notation).

$$
2~\mathrm{B_B^x} + \mathrm{O_O^x} + \mathrm{C_2O_3} \rightarrow 2~\mathrm{C_B'} + \mathrm{v_O^{\bullet\bullet}} + 2~\mathrm{BO_2}
\tag{2}
$$

Furthermore, there are often cases where a two step process is required. For example, the perovskite from reaction (2) could be introduced to a wet atmosphere to partially hydrate the material which creates interstitial proton defects $[\mathrm{OH_O^\bullet}]$ around oxygen positions and removes previously create oxygen vacancies according to the equilibrium reaction in (3). Obviously, this reaction can only take place after the oxygen vacancies have been created.

$$
\mathrm{O_O^x} + \mathrm{v_O^{\bullet \bullet}} + \mathrm{H_2O(g)} \rightleftharpoons 2~[\mathrm{OH_O^\bullet}]
\tag{3}
$$

To allow the user to model these requirements, Mocassin offers two lattice model objects that describe the doping process:

1. **Occupation exchange**
   - Which particle on which lattice site can potentially be replaced by which other particle?
2. **Doping**
   - Which dopings with main exchange and dependent exchange exists and in which order are they processed?  

### [The default building block](#the-default-building-block)

The default building block describes the default occupation of the unit cell to which all dopings are applied. In the current version of Mocassin this object is readonly and the default occupation of each stable site equals the first particle in the affiliated particles set and the void particle for unstable sites.

### [Occupation exchange](#occupation-exchange)

Occupation exchange objects are basically building blocks for a doping. They describe three important properties: (i) the unit cell site they apply to; (ii) the particle they introduce into the lattice; and (iii) the particle that is replaced when the exchange is applied. The serve as an option pool of reusable definitions and have no effect and cannot be quantified as standalone objects.

### [Doping](#doping)

Doping objects define which changes can be quantified for the lattice, in which order they are processed, and if they have a secondary effect. These objects consist of two occupation exchanges, where on is the primary exchange that is quantified and the second is the dependent exchange that is automatically quantified by charge compensation. If both exchanges are identical or the flag for automatic charge compensation is left at "false", then only the primary exchange is applied to the supercell. The order of application is defined by assigning an integer to each doping. The doping objects are applied in ascending order according to these numbers.

## Model examples

### [Ceria](#ceria)

To reduce ceria and create polarons and oxygen vacancies, the following model definitions are required:

1. An occupation exchange that replaces $\mathrm{Ce^{4+}}$ by $\mathrm{Ce^{3+}}$ on the cerium site
2. An occupation exchange that replaces $\mathrm{O^{2-}}$ by $\mathrm{v_O}$ on an oxygen site
3. A doping object that uses the cerium site exchange as the primary exchange, has the oxygen site exchange as the dependent exchange, and uses automatic charge balancing

### [Yttrium doped barium zirconate](#yttrium-doped-barium-zirconate)

To create yttrium doped barium zirconate with partial hydration, the following model definitions are required:

1. An occupation exchange that replaces $\mathrm{Zr^{4+}}$ by $\mathrm{Y^{3+}}$ on the zirconium site
2. An occupation exchange that replaces $\mathrm{O^{2-}}$ by $\mathrm{v_O}$ on the oxygen site
3. A doping object that uses the zirconium site exchange as the primary exchange, has the oxygen site exchange as the dependent exchange, and uses automatic charge balancing
4. An occupation exchange that replaces interstitial $\mathrm{v_i}$ by $\mathrm{H^{+}}$ on the interstitial site
5. An occupation exchange that replaces $\mathrm{v_O}$ by $\mathrm{O^{2-}}$ on the oxygen site
6. A doping object that uses the oxygen restore exchange as the primary exchange, has the proton interstitial site exchange as the dependent exchange, and uses automatic charge balancing