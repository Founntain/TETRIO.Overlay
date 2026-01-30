using Tetrio.Foxhole.Database.Enums;

namespace Tetrio.Zenith.DailyChallenge.ChallengeGeneration;

public class ChallengeScaling
{
    public double Pps { get; set; } = 1;
    public double Apm { get; set; } = 1;
    public double Vs { get; set; } = 1;
    public double Quads { get; set; } = 1;
    public double Spins { get; set; } = 1;
    public double BackToBack { get; set; } = 1;
    public double App { get; set; } = 1;
}

public static class Scaling
{
    // Height doesn't have a constant scaling percentage, as this is based on the current world record

    public static readonly Dictionary<ConditionType, double> NoHoldScaling = new()
    {
        //Type,                      Multiplicative scaling
        { ConditionType.Pps,         1    },
        { ConditionType.Apm,         0.75 },
        { ConditionType.Vs,          0.75 },
        { ConditionType.Quads,       0.8  },
        { ConditionType.Spins,       0.8  },
        { ConditionType.BackToBack,  0.8  },
        { ConditionType.App,         0.8  },
    };

    public static readonly Dictionary<ConditionType, double> MessyScaling = new()
    {
        //Type,                      Multiplicative scaling
        { ConditionType.Pps,         1    },
        { ConditionType.Apm,         0.9  },
        { ConditionType.Vs,          0.85 },
        { ConditionType.Quads,       0.9  },
        { ConditionType.Spins,       1    },
        { ConditionType.BackToBack,  1    },
        { ConditionType.App,         1    },
    };

    public static readonly Dictionary<ConditionType, double> GravityScaling = new()
    {
        //Type,                      Multiplicative scaling
        { ConditionType.Pps,         1    },
        { ConditionType.Apm,         0.75 },
        { ConditionType.Vs,          0.75 },
        { ConditionType.Quads,       1    },
        { ConditionType.Spins,       0.9  },
        { ConditionType.BackToBack,  0.9  },
        { ConditionType.App,         0.95 },
    };

    public static readonly Dictionary<ConditionType, double> VolatileScaling = new()
    {
        //Type,                      Multiplicative scaling
        { ConditionType.Pps,         1    },
        { ConditionType.Apm,         1.1  },
        { ConditionType.Vs,          1.1  },
        { ConditionType.Quads,       1.25 },
        { ConditionType.Spins,       1    },
        { ConditionType.BackToBack,  1    },
        { ConditionType.App,         1.1  },
    };

    public static readonly Dictionary<ConditionType, double> DoubleHoleScaling = new()
    {
        //Type,                      Multiplicative scaling
        { ConditionType.Pps,         1    },
        { ConditionType.Apm,         0.85 },
        { ConditionType.Vs,          0.75 },
        { ConditionType.Quads,       0.8  },
        { ConditionType.Spins,       0.9  },
        { ConditionType.BackToBack,  0.9  },
        { ConditionType.App,         1    },
    };

    public static readonly Dictionary<ConditionType, double> InvisibleScaling = new()
    {
        //Type,                      Multiplicative scaling
        { ConditionType.Pps,         0.7  },
        { ConditionType.Apm,         0.7  },
        { ConditionType.Vs,          0.7  },
        { ConditionType.Quads,       1    },
        { ConditionType.Spins,       0.8  },
        { ConditionType.BackToBack,  0.8  },
        { ConditionType.App,         0.9  },
    };

    public static readonly Dictionary<ConditionType, double> AllSpinScaling = new()
    {
        //Type,                      Multiplicative scaling
        { ConditionType.Pps,         0.8  },
        { ConditionType.Apm,         1.1  },
        { ConditionType.Vs,          1.1  },
        { ConditionType.Quads,       1    },
        { ConditionType.Spins,       1.2  },
        { ConditionType.BackToBack,  1.15 },
        { ConditionType.App,         1.2  },
    };
}