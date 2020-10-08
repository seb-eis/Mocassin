# The structure model

## Description

The structure model controls the symmetry and unit cell definition of the simulation system. It is important to define only required components here to avoid bloating the model. Especially, the definition of unit cell sites that are neither involved in transitions nor show non-zero interactions reduces simulation performance at no benefit.

## The structure model objects

### [Basics](#basics)

The unit cell structure in Mocassin is based on a fixed position approach with space group symmetry, that is, it uses an idealized geometry where symmetry breaks are introduced by position occupation/state changes. From a processing and modelling perspective this is convenient as it allows to create a symmetry-reduced model with highly customizable process and interaction definitions.

Importantly, a user should remember that any geometric 3D vector $\vec{x_i}$ is provided to the program in the affine coordinate context of the unit cell, with the cell vectors $\vec{a},\vec{b},\vec{c}$ forming the axis of the coordinate system. Assuming row vector notation, the 4 x 4 affine transformation matrices to transform from the unit cell system to the cartesian system and vise-versa in homogeneous coordinates are $M_\mathrm{tc}$ and $M_\mathrm{ta}$, respectively, which are obtained as shown in (1).

$$
\begin{aligned}
    M_\mathrm{tc} &=
    \begin{pmatrix}
        a_x & a_y & a_z & 0 \\
        b_x & b_y & b_z & 0 \\
        c_x & c_y & c_z & 0 \\
        0 & 0 & 0 & 1
    \end{pmatrix}\\
    M_\mathrm{ta} &= (M_\mathrm{tc})^{-1}
\end{aligned}
\tag{1}
$$

In Mocassin, the affine coordinate space of the unit cell is called "fractional coordinates" where all vectors with components $a,b,c \in [0...1)$ lie within the unit cell. This is in contrast to typical definitions where fractional coordinates refer to a supercell coordinate space. A vector with entries $a,b,c \ge 1$ is valid in most geometric definitions that describe connected points, it just means that the sequence spans over more than a single unit cell. It should ne noted that Mocassin makes full use of the translation invariance of the system internally, that is, the readable but inefficient 3D vector description is transformed into an efficient but cryptic 4D integer vector $(z_a,z_b,z_c,z_p)$ space for simulation.

Overall, the structure model defines several model objects and parameters to define the geometry and symmetry of the unit cell:

1. **Space Group**
   - What symmetry operations exist?
2. **Cell Parameters**
   - What are the lengths and angles of the unit cell?
3. **Unit Cell Position** (*Unit cell position*)
   - What are the stable and unstable reference positions of the unit cell?

### [Space group](#space-group)

The space group is a unique model object that defines the symmetry operations of the unit cell and is selected from a small SQLite database shipped with the software. The database contains at least one entry for each of the 230 space groups and in some cases with multiple equivalent origin choices or unique axis options, more than one option is provided.

Each symmetry operation is noted in an $"x,y,z"$ style which actually refers to the affine coordinate space of the unit cell and not the cartesian space, that is, $(x,y,z) \equiv (a,b,c)$. They describe an Euclidean mapping, that is, an affine transformation preserving all distances and angles, for each vector $\vec{x} = (x,y,z)$ into a new one $\vec{x}' = (x',y',z')$ that is symmetry equivalent to the original. The general notation is shown in (2), where $w_i$ is the translation component added to axis $i$ of $\vec{x}'$ and each $c_{ij}$ describes the the contribution of the original axis $j$ of $\vec{x}$ to the axis $i$ of the mapped vector $\vec{x}'$. In many cases most values of $w_i$ and $c_{ij}$ are zero and thus the "x,y,z" style notation is very compact.

$$
\begin{aligned}
    x' &= c_{xx} x + c_{xy} y + c_{xz} z + w_x \\
    y' &= c_{yx} x + c_{yy} y + c_{yz} z + w_y \\
    z' &= c_{zx} x + c_{zy} y + c_{zz} z + w_z
\end{aligned}
\tag{2}
$$

For mathematical purposes the $"x,y,z"$ notation is not helpful and can be translated into a general 4 x 4 affine transformation matrix $T_\mathrm{s}$ for [homogeneous coordinates](https://en.wikipedia.org/wiki/Homogeneous_coordinates) with vectors $(x,y,z,1)$. Assuming a row vector layout, this leads to a transformation logic as shown in (3). It is also possible to calculate the composed matrix transform $M_\mathrm{ta} T_\mathrm{s} M_\mathrm{tc}$, which allows to apply the symmetry operation to a cartesian vector directly.

$$
\begin{aligned}
    \vec{x}' &= \vec{x} \cdot T_\mathrm{s} = \vec{x} \cdot 
    \begin{pmatrix}
        c_{xx} & c_{yx} & c_{zx} & 0 \\
        c_{xx} & c_{yx} & c_{zx} & 0 \\
        c_{xx} & c_{yx} & c_{zx} & 0 \\
        w_x & w_y & w_z & 1
    \end{pmatrix} 
\end{aligned}
\tag{3}
$$

### [Cell parameters](#cell-parameters)

The cell parameters are a unique model object that defines the cell geometry by axis lengths $a,b,c$ and the cell angles $\alpha,\beta,\gamma$. The crystal system and consequently the parameter boundaries are predefined by the selected space group and thus some of the parameters are automatically set by the system. For example, if a space group with a cubic crystal system is selected, only the parameter $a$ can be modified.

It is important to note is that it is a common misconception that a triclinic cell can have any angle combination as long as $\alpha, \beta, \gamma \in (0...2\pi)$ is satisfied. There are four additional conditions that need to be satisfied for the angles to form a valid crystal geometry as shown in (4). The rules can be easily remembered: "No angle can be bigger than the sum of the remaining two and the sum of all angles cannot equal 0°."

$$
\begin{aligned}
    0 &\lt \alpha + \beta + \gamma \lt 2 \pi \\
    0 &\lt \alpha + \beta - \gamma \lt 2 \pi \\
    0 &\lt \alpha - \beta + \gamma \lt 2 \pi \\
    0 &\lt \beta + \gamma - \alpha \lt 2 \pi
\end{aligned}
\tag{4}
$$

### [Unit cell sites](#unit-cell-sites)

The unit cell site or position model objects describes a reference lattice position of the unit cell. It is basically equivalent to a Wyckoff position that additionally defines a set of possible occupation and a stability information. The position object is defined by its coordinate info $a,b,c \in [0...1)$, a previously defined particle set that describes the occupation, and the stability flag to mark *stable* or *unstable* sites. The defined sites are symmetry extended with the space group during processing. Sites that are marked as *unstable* are required to describe transition sites for KMC simulations, that is, pure MMC problem should only define *stable* sites.

It is important to avoid unnecessary site definitions as even though Mocassin will detect and optimize dead interactions during simulation startup, the system cannot retroactively remove dead lattice positions. This increase memory consumption and model fragmentation, both will decrease performance of the solver compared to a clean model.

## Model examples

### [Ceria](#ceria)

The following model definitions are required to define ceria $\mathrm{CeO_2}$ and later allow oxygen migration by KMC:
1. Select space group 225, i.e. $Fm \overline{3} m$
2. Set the cell parameter $a$, e.g. to $5.41~\mathrm{\mathring{A}}$
3. A stable cerium site, e.g. at $(0,0,0)$
4. A stable oxygen site, e.g. at $(0.25,0.25,0.25)$
5. An unstable oxygen migration site, e.g. at $(0.50,0.25,0.25)$