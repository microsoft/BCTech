# E-Document Connector Workshop
## Directions EMEA 2025

Welcome to the E-Document Connector Workshop! This hands-on workshop teaches you how to build complete E-Document integrations using the Business Central E-Document Core framework.

---

## 🎯 Workshop Overview

**Duration**: 90 minutes (10 min intro + 80 min hands-on)

**What You'll Build**:
1. **SimpleJson Format** - E-Document format implementation for JSON documents
2. **DirectionsConnector** - HTTP API integration for sending/receiving documents
3. **Complete Round-Trip** - Full document exchange between Business Central and external systems

**What You'll Learn**:
- How the E-Document Core framework works
- How to implement format interfaces
- How to implement integration interfaces
- Best practices for E-Document solutions

---

## 📂 Workshop Structure

```
DirectionsEMEA2025/
├── WORKSHOP_INTRO.md          # 📊 VS Code presentation (10-minute intro)
├── WORKSHOP_GUIDE.md          # 📘 Step-by-step exercises with solutions
├── API_REFERENCE.md           # 🔌 Complete API documentation
├── WORKSHOP_PLAN.md           # 📝 Detailed implementation plan
│
├── application/
│   ├── simple_json/           # Exercise 1: Format Extension
│   │   ├── app.json
│   │   ├── SimpleJsonFormat.EnumExt.al
│   │   ├── SimpleJsonFormat.Codeunit.al       # ⚠️ TODO sections
│   │   └── SimpleJsonHelper.Codeunit.al       # ✅ Pre-written
│   │
│   └── directions_connector/  # Exercise 2: Integration Extension
│       ├── app.json
│       ├── DirectionsIntegration.EnumExt.al
│       ├── DirectionsIntegration.Codeunit.al  # ⚠️ TODO sections
│       ├── DirectionsConnectionSetup.Table.al # ✅ Pre-written
│       ├── DirectionsConnectionSetup.Page.al  # ✅ Pre-written
│       ├── DirectionsAuth.Codeunit.al         # ✅ Pre-written
│       └── DirectionsRequests.Codeunit.al     # ✅ Pre-written
│
├── solution/                  # 📦 Complete working solution (instructor)
│   ├── simple_json/
│   └── directions_connector/
│
└── server/                    # 🐍 Python API server (Azure hosted)
    ├── server.py
    ├── requirements.txt
    └── README.md
```

---

## 🚀 Getting Started

### For Participants

1. **Read the Introduction** - Open `WORKSHOP_INTRO.md` in VS Code (Ctrl+Shift+V for preview)
2. **Follow the Guide** - Use `WORKSHOP_GUIDE.md` for step-by-step instructions
3. **Reference the API** - Check `API_REFERENCE.md` for endpoint details
4. **Ask Questions** - The instructor is here to help!

### For Instructors

1. **Review the Plan** - See `WORKSHOP_PLAN.md` for complete overview
2. **Present the Intro** - Use `WORKSHOP_INTRO.md` as your slide deck in VS Code
3. **Provide API URL** - Update the API Base URL in materials before workshop
4. **Reference Solution** - Complete implementation in `/solution/` folder
5. **Monitor Progress** - Check `/peek` endpoint to see participant submissions

---

## ⏱️ Workshop Timeline

| Time | Duration | Activity | File |
|------|----------|----------|------|
| 00:00-00:10 | 10 min | Introduction & Architecture | WORKSHOP_INTRO.md |
| 00:10-00:40 | 30 min | Exercise 1: SimpleJson Format | WORKSHOP_GUIDE.md |
| 00:40-01:10 | 30 min | Exercise 2: DirectionsConnector | WORKSHOP_GUIDE.md |
| 01:10-01:25 | 15 min | Testing & Live Demo | WORKSHOP_GUIDE.md |
| 01:25-01:30 | 5 min | Wrap-up & Q&A | - |

---

## 📋 Prerequisites

### Required
- Business Central development environment (Sandbox or Docker)
- VS Code with AL Language extension
- Basic AL programming knowledge
- API Base URL (provided by instructor)

### Helpful
- Understanding of REST APIs
- JSON format familiarity
- Postman or similar API testing tool

---

## 🎓 Learning Objectives

By the end of this workshop, participants will be able to:

1. ✅ Understand the E-Document Core framework architecture
2. ✅ Implement the "E-Document" interface for custom formats
3. ✅ Implement IDocumentSender and IDocumentReceiver interfaces
4. ✅ Configure and use E-Document Services in Business Central
5. ✅ Test complete document round-trips (send and receive)
6. ✅ Troubleshoot common integration issues
7. ✅ Know where to find resources for further learning

---

## 📚 Key Concepts

### E-Document Format Interface
Converts Business Central documents to/from external formats:
- **Outgoing**: `Check()`, `Create()`, `CreateBatch()`
- **Incoming**: `GetBasicInfoFromReceivedDocument()`, `GetCompleteInfoFromReceivedDocument()`

### Integration Interfaces
Communicates with external systems:
- **IDocumentSender**: `Send()` - Send documents
- **IDocumentReceiver**: `ReceiveDocuments()`, `DownloadDocument()` - Receive documents
- **IDocumentResponseHandler**: `GetResponse()` - Async status (advanced)
- **ISentDocumentActions**: Approval/cancellation workflows (advanced)

### SimpleJson Format
- Simple JSON structure for learning
- Header fields: document type, number, customer, date, amount
- Lines array: line items with quantities and prices
- Human-readable and easy to debug

### DirectionsConnector
- REST API integration via HTTP
- Authentication via API key header
- Queue-based document exchange
- Stateless and scalable

---

## 🧪 Testing Scenarios

### Solo Testing
1. Send documents from your BC instance
2. Verify in API queue via `/peek`
3. Receive documents back into BC
4. Create purchase invoices

### Partner Testing
1. Partner A sends documents
2. Partner B receives and processes
3. Swap roles and repeat
4. Great for testing interoperability!

### Group Testing
- Multiple participants send to same queue
- Everyone receives mixed documents
- Tests error handling and validation

---

## 🐛 Troubleshooting

See the **Troubleshooting** section in `WORKSHOP_GUIDE.md` for:
- Connection issues
- Authentication problems
- JSON parsing errors
- Document creation failures

Common solutions provided for all scenarios!

---

## 📖 Additional Resources

### Workshop Materials
- [Workshop Introduction](WORKSHOP_INTRO.md) - Slide deck presentation
- [Workshop Guide](WORKSHOP_GUIDE.md) - Detailed instructions with code
- [API Reference](API_REFERENCE.md) - Complete endpoint documentation
- [Workshop Plan](WORKSHOP_PLAN.md) - Implementation overview

### E-Document Framework
- [E-Document Core README](../../NAV_1/App/BCApps/src/Apps/W1/EDocument/App/README.md) - Official framework documentation
- [E-Document Interface](../../NAV_1/App/BCApps/src/Apps/W1/EDocument/App/src/Document/Interfaces/EDocument.Interface.al) - Interface source code

### External Resources
- [Business Central Documentation](https://learn.microsoft.com/dynamics365/business-central/)
- [AL Language Reference](https://learn.microsoft.com/dynamics365/business-central/dev-itpro/developer/devenv-reference-overview)
- [FastAPI Documentation](https://fastapi.tiangolo.com/) - For the server implementation

---

## 🎁 Bonus Content

After completing the workshop, try these challenges:

### Format Enhancements
- Add support for Credit Memos
- Implement batch processing (`CreateBatch()`)
- Add custom field mappings
- Support attachments (PDF, XML)

### Integration Enhancements
- Implement `IDocumentResponseHandler` for async status
- Add retry logic and error handling
- Implement `ISentDocumentActions` for approvals
- Add custom actions with `IDocumentAction`

### Real-World Applications
- Connect to actual PEPPOL networks
- Integrate with Avalara or other tax services
- Build EDI integrations
- Create custom XML formats for local requirements

---

## 🤝 Contributing

This workshop is part of the BCTech repository. If you have suggestions or improvements:

1. Open an issue on GitHub
2. Submit a pull request
3. Share your feedback with the instructor

---

## 📄 License

This workshop material is provided under the MIT License. See the repository root for details.

---

## 🙏 Acknowledgments

- **Microsoft Business Central Team** - For the E-Document Core framework
- **BCTech Community** - For continuous contributions
- **Workshop Participants** - For your enthusiasm and feedback!

---

## 📞 Support

### During the Workshop
- Ask the instructor
- Check the WORKSHOP_GUIDE.md
- Collaborate with neighbors

### After the Workshop
- GitHub Issues: [BCTech Repository](https://github.com/microsoft/BCTech)
- Documentation: [Learn Microsoft](https://learn.microsoft.com/dynamics365/business-central/)
- Community: [Business Central Forums](https://community.dynamics.com/forums/thread/)

---

## 🎯 Quick Start

**Ready to begin? Follow these steps:**

1. ✅ Open `WORKSHOP_INTRO.md` and read through the introduction
2. ✅ Get the API Base URL from your instructor
3. ✅ Open `WORKSHOP_GUIDE.md` and start Exercise 1
4. ✅ Have fun building your E-Document integration!

---

**Happy Coding!** 🚀

*For questions or feedback, contact the workshop instructor.*
