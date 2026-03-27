# BrainFlow Biosignals for Unity

Real-time biosignal streaming from OpenBCI hardware to Unity via BrainFlow.

## Overview

This plugin bridges BrainFlow's biosignal processing capabilities with Unity, enabling:
- **EMG** (muscle activity) → control objects, trigger events
- **EEG** (brain signals) → mental state detection
- **FSR** (pressure sensors) → force/pressure input

## Architecture

```
[OpenBCI Hardware] → [BrainFlow] → [BrainFlowManager] → [SignalProcessor] → [Your Game Logic]
```

## Core Components

### BrainFlowManager
Handles board connection and raw data streaming.

### SignalProcessor  
Normalizes, smooths, and thresholds signals.

### BiosignalInput
High-level API similar to Unity's Input system.

## Quick Start

```csharp
// Check if user is flexing
if (biosignalInput.IsFlexing)
{
    // Do something
}

// Get continuous EMG value (0-1)
float emg = biosignalInput.EMGValue;
transform.position = new Vector3(0, emg * 5f, 0);
```

## Supported Boards

- Synthetic Board (ID: -1) - for testing without hardware
- OpenBCI Cyton (ID: 0)
- OpenBCI Ganglion (ID: 1)
- OpenBCI Cyton + Daisy (ID: 2)
- See [BrainFlow docs](https://brainflow.readthedocs.io/en/stable/SupportedBoards.html) for full list
