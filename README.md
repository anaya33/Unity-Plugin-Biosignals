# BrainFlow Biosignals for Unity

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Unity 2021.3+](https://img.shields.io/badge/Unity-2021.3%2B-blue.svg)](https://unity.com/)

**Real-time biosignal streaming from OpenBCI hardware to Unity via BrainFlow.**

Control your Unity/XR experiences with EMG (muscle), EEG (brain), and FSR (pressure) signals.

## 🎯 What This Does

```
[OpenBCI Hardware] → [BrainFlow] → [Unity Plugin] → [Your Game/XR App]
```

Instead of keyboard/controller input, use your body:
- **Flex muscle** → move object, trigger action
- **Brain signals** → detect mental states
- **Pressure sensors** → measure force precision

## 🚀 Quick Start

### Installation

1. **Install BrainFlow** in your Unity project:
   - Download BrainFlow C# bindings from [brainflow.org](https://brainflow.org)
   - Add the DLLs to your `Assets/Plugins` folder

2. **Add this package** via Unity Package Manager:
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
        // Simple: is the user flexing?
        if (biosignalInput.IsFlexing)
        {
            Jump();
        }

        // Continuous: get EMG value 0-1
        float emg = biosignalInput.EMGValue;
        transform.position = new Vector3(0, emg * 5f, 0);
    }
}
```

## 📦 Package Structure

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

## 🔧 Configuration

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
- **Normalization**: Raw values → 0-1 range
- **Smoothing**: Reduces noise via moving average
- **Thresholding**: Defines "active" vs "inactive"
- **Calibration**: Auto-detect your signal range

## 🎮 Sample: EMG Cube Control

Import the sample from Package Manager:
1. Find "BrainFlow Biosignals" in Package Manager
2. Expand "Samples"
3. Click "Import" on "EMG Cube Control"

**Result**: Flex your muscle → cube moves up. Release → cube falls.

## 🧪 Testing Without Hardware

Use the **Synthetic Board** (Board ID: -1) to test without OpenBCI hardware. It generates fake biosignal data so you can develop your integration.

## 📚 API Reference

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

## 🔮 Roadmap

- [ ] EEG band power extraction (alpha, beta, etc.)
- [ ] FSR pressure mapping
- [ ] Multi-channel visualization
- [ ] Unity XR Toolkit integration
- [ ] Recording & playback

## 🤝 Contributing

PRs welcome! This is an open-source project aimed at making biosignal-driven XR more accessible.

## 📄 License

MIT License - see [LICENSE](LICENSE)

## 🔗 Links

- [BrainFlow Documentation](https://brainflow.readthedocs.io/)
- [OpenBCI](https://openbci.com/)
