# BCTalent.EscapeRoom - Framework Documentation

## Overview

The **BCTalent.EscapeRoom** app is an educational framework for creating interactive "escape room" experiences within Microsoft Business Central. It enables developers and consultants to learn Business Central concepts through hands-on, gamified challenges that are validated in real-time.

The framework provides a complete infrastructure for:
- Creating themed learning venues with multiple rooms
- Defining tasks with automatic validation
- Tracking progress with telemetry and scoring
- Building leaderboards for competitive learning events
- Managing hints and solutions for facilitators

**Current Version:** 1.3.10026.0  
**ID Range:** 73920-73999  
**Publisher:** waldo & AJ

---

## Documentation Structure

### 📚 [Framework Documentation](Docs/Framework/)

Core framework architecture and patterns for developers extending the system:
- [Architecture Overview](Docs/Framework/Architecture.md) - Core components and design patterns
- [Creating New Rooms](Docs/Framework/Creating-Rooms.md) - Step-by-step guide for building rooms
- [Task Validation System](Docs/Framework/Task-Validation.md) - How task validation and interfaces work
- [Telemetry Integration](Docs/Framework/Telemetry-Integration.md) - Scoring, tracking, and Application Insights

### 🎯 [Facilitator Documentation](Docs/Facilitator/)

Guides for event organizers running escape room sessions:
- [Event Setup Guide](Docs/Facilitator/Event-Setup.md) - Provisioning environments and publishing apps
- [Leaderboard Setup](Docs/Facilitator/Leaderboard-Setup.md) - Configuring Application Insights dashboards
- [Participant Management](Docs/Facilitator/Participant-Management.md) - User setup and permissions

### 🔧 [Developer Reference](Docs/Dev/)

Technical documentation for framework maintainers:
- [Developer Guide](Docs/Dev/README.md) - Entry point for technical documentation
- [API Reference](Docs/Dev/API-Reference.md) - Interfaces and extension points
- [Permission Sets](Docs/Dev/Permissions.md) - Security configuration

### 📋 [Changelog](Docs/CHANGELOG.md)

Complete chronological history of all changes to the framework across all development phases.

---

## Quick Start

### For Facilitators
1. Read [Event Setup Guide](Docs/Facilitator/Event-Setup.md) to prepare environments
2. Configure [Leaderboard Setup](Docs/Facilitator/Leaderboard-Setup.md) for tracking
3. Review [Participant Management](Docs/Facilitator/Participant-Management.md) for user setup

### For Developers Creating Rooms
1. Understand the [Architecture Overview](Docs/Framework/Architecture.md)
2. Follow [Creating New Rooms](Docs/Framework/Creating-Rooms.md) guide
3. Implement [Task Validation System](Docs/Framework/Task-Validation.md) for challenges
4. Integrate [Telemetry](Docs/Framework/Telemetry-Integration.md) for scoring

### For Framework Contributors
1. Review [Developer Guide](Docs/Dev/README.md)
2. Study [API Reference](Docs/Dev/API-Reference.md)
3. Check [Changelog](Docs/CHANGELOG.md) for recent updates

---

## Key Concepts

**Venue** → A collection of themed rooms (e.g., "Development Track", "Consultant Track")  
**Room** → An individual learning challenge within a venue  
**Task** → A specific validation step within a room that participants must complete  
**Interface-Based Extension** → Other apps extend by implementing framework interfaces  
**Automatic Validation** → Tasks can self-validate using the `IEscape Room Task Validation` interface  
**Telemetry-Driven Scoring** → All progress tracked via Application Insights with point system

---

## Related Projects

This framework is extended by:
- **Development.1** - First development-focused escape room venue
- **Development.2** - AI/Copilot translation-focused escape room venue
- **Consultant.1** - Business Central functionality-focused venue

See each app's documentation for venue-specific room details.

---

## Support & Contributions

For questions, issues, or contributions related to the framework, see the project repository.

**Last Updated:** January 7, 2026
