# BrainFlow Biosignals for Unity

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Unity 2021.3+](https://img.shields.io/badge/Unity-2021.3%2B-blue.svg)](https://unity.com/)

A Unity plugin that streams real-time biosignal data from OpenBCI hardware via BrainFlow. Use EMG, EEG, and pressure signals as input for games and XR applications.

## What This Does

```
[OpenBCI Hardware] → [BrainFlow] → [Unity Plugin] → [Your Game/XR App]
```

Instead of keyboard or controller input, you can use your body:
- Muscle activation (EMG) to move objects or trigger events
- Brain signals (EEG) to detect mental states
- Pressure sensors (FSR) to measure force and precision

## Quick Start

### Installation

1. Install BrainFlow in your Unity project:
   - Download BrainFlow C# bindings from [brainflow.org](https://brainflow.org)
   - Add the DLLs to your `Assets/Plugins` folder

2. Add this package via Unity Package Manager:
   - Window → Package Manager → + → Add package from git URL
   - Enter: `https://github.com/anaya33/Unity-Plugin-Biosignals.git`

### Basic Usage

```csharp
using BrainFlowBiosignals;

public class MyController : MonoBehaviour
{
    [SerializeField] private BiosignalInput biosignalInput;

    void Update()
    {
        // Check if user is flexing
        if (biosignalInput.IsFlexing)
        {
            Jump();
        }

        // Or use the continuous EMG value (0-1)
        float emg = biosignalInput.EMGValue;
        transform.position = new Vector3(0, emg * 5f, 0);
    }
}
```

## Package Structure

```
├── Runtime/
│   ├── BrainFlowManager.cs      # Board connection & streaming
│   ├── SignalProcessor.cs       # Normalization & smoothing
│   └── BiosignalInput.cs        # High-level input API
├── Editor/
│   └── BrainFlowManagerEditor.cs
├── Samples~/
│   └── EMGCubeControl/          # Flex muscle → cube moves up
└── Documentation~/
```

## Configuration

### Board Setup

In the Inspector, configure `BrainFlowManager`:

| Board ID | Hardware |
|----------|----------|
| -1 | Synthetic (testing, no hardware needed) |
| 0 | OpenBCI Cyton |
| 1 | OpenBCI Ganglion |
| 2 | Cyton + Daisy |

### Signal Processing

`SignalProcessor` handles:
- **Normalization** - converts raw values to 0-1 range
- **Smoothing** - reduces noise via moving average
- **Thresholding** - defines when a signal counts as "active"
- **Calibration** - auto-detects your personal signal range

## Sample: EMG Cube Control

Import the sample from Package Manager:
1. Find "BrainFlow Biosignals" in Package Manager
2. Expand "Samples"
3. Click "Import" on "EMG Cube Control"

Flex your muscle and the cube moves up. Release and it falls back down.

## Testing Without Hardware

Use the Synthetic Board (Board ID: -1) to test without any OpenBCI hardware. It generates fake biosignal data so you can develop and test your integration before connecting real sensors.

## API Reference

### BiosignalInput

| Property | Type | Description |
|----------|------|-------------|
| `EMGValue` | float | Smoothed EMG value (0-1) |
| `EMGValueRaw` | float | Raw normalized value |
| `IsFlexing` | bool | True if above threshold |
| `FlexStrength` | float | Strength above threshold (0-1) |

| Event | Description |
|-------|-------------|
| `OnFlexStart` | Fired when flex begins |
| `OnFlexEnd` | Fired when flex ends |

### BrainFlowManager

| Method | Description |
|--------|-------------|
| `StartStreaming()` | Begin data stream |
| `StopStreaming()` | Stop data stream |
| `GetNormalizedEMG(channel)` | Get single EMG value |

## Roadmap

- [ ] EEG band power extraction (alpha, beta, etc.)
- [ ] FSR pressure mapping
- [ ] Multi-channel visualization
- [ ] Unity XR Toolkit integration
- [ ] Recording & playback

## Contributing

Pull requests welcome. This is an open-source project aimed at making biosignal-driven XR more accessible.

## License

MIT License - see [LICENSE](LICENSE)

## Links

- [BrainFlow Documentation](https://brainflow.readthedocs.io/)
- [OpenBCI](https://openbci.com/)
