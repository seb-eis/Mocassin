# Movement tracking during simulation

## Basics

The simulator tracks and stores particle ensemble movements during simulation with three distinct concepts: (i) transition-based or "global" tracking; (ii) particle-based or "mobile" tracking; and (iii) position-based or "static" tracking. In contrast to common molecular dynamics software, tracking of the entire trajectory set as a function of time so that there is a $\vec{x}_i(t)$ set for every particle after simulation is not done. The available tracking systems just offer most of the data relevant to evaluation of conductivity, mobility, and diffusion at greatly reduced disk space, rendering them the superior option for large scale sampling. 

**Note:** The access of raw tracking data requires the C# evaluation API for reading the binary state files. The stdout text dumps contain only inferred properties, such as mobility, conductivity, mean square displacement, and mean displacement of ensembles.

## The tracking systems

### [Transition-based tracking](#transition-based-tracking)

The transition-based or "global" tracking is the simplest movement tracing method used in Mocassin. It contains a 2D set of ensemble displacement vectors $\Delta \vec{x}_{ij}$ that store data for each species $i$ and transition model object $j$. During simulation, when a transition type $j$ is executed, each particle of species $i$ causes adding of its displacement $\Delta \vec{x}_i$ to the affiliated tracking vector. This means, these "global" trackers do not contain individual displacement of particles, but allow to calculate the average ensemble displacement of particles that was caused by a specific type of transition $\langle \Delta \vec{x}_{ij} \rangle$, that is, they allow to separate the contributions to conductivity $\sigma_{ij}$ and mobility $u_{ij}$ from different transport processes by using equations (1) and (2), respectively. Here, $\vec{E}$ is the electric field, $t$ is the simulated time, $q_i$ is the charge of the species type $i$, and $c_i$ is the particle density.

$$
u_{ij} = \frac{\langle \Delta \vec{x}_{ij} \rangle \cdot \vec{E}}{|\vec{E}|^2 \cdot t}
\tag{1}
$$

$$
\sigma_{ij} = u_{ij} \cdot q_i \cdot c_i
\tag{2}
$$

### [Particle-based tracking](#particle-based-tracking)

The particle-based or "mobile" tracking assigns each mobile particle, that is, it can potentially leave its initial position, an individual displacement vector $\Delta \vec{x}$. At the end of a simulation, the system thus stores the individual displacements off all particles of the supercell without the history/trajectory of each object. While the data can be used for conductivity and mobility calculation, the main purpose of the trackers is to calculate mean square displacement data $\langle \Delta x_{i}^2 \rangle$ either for different directions $x$ or the radial average, and consequently the affiliated diffusion coefficients $D_i(x)$ and $D_i$ according to expressions (3) and (4). It can also be potentially used to identify "trapped" particles that showed little or no movement during simulation.

$$
D_i(x) = \frac{\langle \Delta x_{i}^2 \rangle}{2 \cdot t}
\tag{3}
$$

$$
D_i = \frac{\langle |\Delta \vec{x}_{i}|^2 \rangle}{6 \cdot t}
\tag{4}
$$

### [Position-based tracking](#position-based-tracking)

The position-based or "static" tracking assigns each stable $(a,b,c,p)$ position of the lattice a displacement vector $\Delta \vec{x}_{pi}$ for each possible particle type $i$ that can occupy the position. During simulation, when a transition is executed and a particle type $i$ moves away from a position, its displacement vector due to that transition $\Delta x_i$ is added to the affiliated $\Delta \vec{x}_{pi}$. That means, at the end of the simulation the static tracking system contains the absolute displacement of each particle type $i$ performed on each position $p$. By including the simulated time span $t$, this allows to calculate the magnitude and direction of the outgoing particle flow of each position in the supercell. Thus, fast and slow transport paths in the supercell can be identified and then, for example, affiliated environments can be analyzed or fast migration pathways can be visualized. 