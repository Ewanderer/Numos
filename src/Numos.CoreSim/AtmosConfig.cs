using System.Threading;

namespace Numos.CoreSim;

/// <summary>
///     Configuration values for the simulation.
/// </summary>
public class AtmosConfig
{
    /// <summary>
    ///     List of gases actively registered to the sim.
    /// </summary>
    public IReadOnlyCollection<GasProperties> GasRegistry => _gasRegistryList;

    private readonly HashSet<GasProperties> _gasRegistryList = [];

    private readonly object _registryLockObject = new();

    /// <summary>
    /// Tries to add a new gas to the registry and marks the config as changed.
    /// </summary>
    public bool AddGas(GasProperties newGas){
        lock(_registryLockObject){
            if(_gasRegistryList.Add(newGas)){
                registryVersion++;
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Tries to remove a gas from the config and marks the config as changed.
    /// </summary>
    public bool RemoveGas(GasProperties gas){
        lock(_registryLockObject){
            if(_gasRegistryList.Remove(gas)){
                registryVersion++;
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// A simple counter that is updated if a registry is changed.
    /// This allows us to quickly check if something like the reaction master matrix needs to be recalculated before a simulation step.
    /// </summary>
    public int RegistryVersion { get; private set; }

    /// <summary>
    ///     Reference ambient temperature.
    /// </summary>
    public float GlobalTemperature { get; set; } = 293.15f;

    /// <summary>
    ///     Default fallback temperature to set when a voxel has 0 or an uninitialized temperature.
    /// </summary>
    public float DefaultTemperatureFallback { get; set; } = 293.15f;

    /// <summary>
    ///     Default temperature of space.
    /// </summary>
    public float SpaceTemperature { get; set; } = 2.7f;

    /// <summary>
    ///     Fraction of pressure delta converted to flow per tick.
    /// </summary>
    public float FlowFriction { get; set; } = 0.25f;

    /// <summary>
    ///     Multiplier applied to <see cref="FlowFriction" /> during large-delta advection.
    ///     Used to reduce oscillation in the sim.
    /// </summary>
    public float DampingFactor { get; set; } = 0.5f;

    /// <summary>
    ///     Below this pressure delta, flow uses the <see cref="CflFlowCap" /> directly
    ///     instead of <see cref="FlowFriction" /> * <see cref="DampingFactor" />
    /// </summary>
    public float SnapThreshold { get; set; } = 5.0f;

    /// <summary>
    ///     Flows below this magnitude are discarded.
    /// </summary>
    public float MinFlowCutoff { get; set; } = 0.1f;

    /// <summary>
    ///     Below this pressure, voxel contents are zeroed out.
    /// </summary>
    public float VacuumThreshold { get; set; } = 1.0f;

    /// <summary>
    ///     Consecutive ticks below <see cref="SleepEpsilon" /> before a chunk goes to sleep.
    /// </summary>
    public int SleepThreshold { get; set; } = 100;

    /// <summary>
    ///     Maximum pressure delta considered "at rest".
    /// </summary>
    public float SleepEpsilon { get; set; } = 3.5f;

    /// <summary>
    ///     Fraction of temperature delta transferred per neighbor per tick.
    /// </summary>
    public float ThermalConductivity { get; set; } = 0.05f;

    /// <summary>
    ///     Rate multiplier for phase-change condensation.
    /// </summary>
    public float CondensationRateFactor { get; set; } = 0.5f;

    /// <summary>
    ///     Rate multiplier for phase-change condensation.
    /// </summary>
    public float CflFlowCap { get; set; } = 0.16f;

    public float AccumulatorWakeThreshold { get; set; } = 15.0f;
    public int AccumulatorMaxAliveTicks { get; set; } = 20;
}