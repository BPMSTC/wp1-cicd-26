-- Purpose: Seed mock sci-fi mission data into dbo.Missions for development/testing.
-- Execution order: Run this script second (02), after 01-create-table.sql.

INSERT INTO dbo.Missions
    (Title, Description, AssignedSector, Commander, Priority, Status, ThreatLevel, LaunchDate)
VALUES
    (N'Operation Nebula Shield', N'Deploy defense satellites to protect the Orion trade corridor from ion storms.', N'Orion Expanse', N'Cmdr. A. Voss', N'High', N'Planned', N'Medium', '2026-05-03'),
    (N'Project Silent Quasar', N'Conduct stealth reconnaissance around a dormant quasar showing anomalous emissions.', N'Vega Frontier', N'Cmdr. N. Reyes', N'Medium', N'In Progress', N'High', '2026-04-19'),
    (N'Titan Relay Recovery', N'Retrieve a lost quantum relay beacon drifting near Titan''s rings.', N'Saturn Outer Rim', N'Cmdr. L. Okafor', N'High', N'In Progress', N'Medium', '2026-04-07'),
    (N'Crimson Dwarf Survey', N'Map habitable moon signatures in a red dwarf system for colony planning.', N'Helios Reach', N'Cmdr. M. Ito', N'Low', N'Planned', N'Low', '2026-06-11'),
    (N'Event Horizon Watch', N'Monitor gravitational lensing around the KX-91 black hole observatory.', N'Perseus Vault', N'Cmdr. S. Mbaye', N'High', N'Planned', N'High', '2026-05-24'),
    (N'Operation Frost Comet', N'Escort cryogenic cargo through pirate-active comet lanes.', N'Andromeda Drift', N'Cmdr. E. Novak', N'Medium', N'Completed', N'Medium', '2026-03-28'),
    (N'Ion Storm Cartography', N'Chart safe jump vectors through recurring ionized storm fronts.', N'Cygnus Passage', N'Cmdr. T. Al-Kindi', N'Medium', N'In Progress', N'Medium', '2026-04-02'),
    (N'Atlas Gate Calibration', N'Recalibrate ancient gate coordinates to reopen deep-space transit routes.', N'Proxima Lattice', N'Cmdr. Y. Tanaka', N'High', N'Blocked', N'High', '2026-04-14'),
    (N'Phantom Signal Trace', N'Investigate repeating non-human transmissions detected near dark space sector 12.', N'Noctis Divide', N'Cmdr. R. Hale', N'High', N'In Progress', N'High', '2026-04-09'),
    (N'Solaris Habitat Drop', N'Deliver modular habitat pods to the Solaris-3 terraforming team.', N'Solaris Belt', N'Cmdr. C. Mensah', N'Low', N'Completed', N'Low', '2026-03-16'),
    (N'Void Runner Escort', N'Escort diplomatic vessel through unstable wormhole approach vectors.', N'Kepler Crossing', N'Cmdr. J. Park', N'Medium', N'Planned', N'Medium', '2026-05-01'),
    (N'Operation Echo Lantern', N'Recover autonomous probes lost after a magnetar flare event.', N'Magnetar Verge', N'Cmdr. D. Ibrahim', N'High', N'Planned', N'High', '2026-05-18');
