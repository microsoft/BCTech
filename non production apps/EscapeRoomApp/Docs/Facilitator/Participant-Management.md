# Participant Management

## Overview

This guide covers onboarding participants for escape room events, managing user access, providing support during events, and handling common participant issues.

---

## Pre-Event: Participant Onboarding

### Step 1: Collect Participant Information

**Required Information:**
- Full Name (for leaderboard display)
- Partner/Company Name (for grouping)
- Email address (for credentials)
- Team assignment (if team-based)

**Optional Information:**
- BC experience level
- Preferred venue track (Development/Consultant)
- Accessibility requirements
- Time zone (for virtual events)

### Step 2: Assign Participants to Environments

**Assignment Strategies:**

**Individual Mode (1 person per environment):**
- Each participant gets dedicated sandbox
- Independent progress and scoring
- Best for competitive events
- Requires more environments

**Team Mode (2 people per environment):**
- Teams share sandbox and progress
- Collaborative learning
- Reduces environment needs
- Encourages peer learning

**Recommended Mapping:**
```
Environment 01 → Participant A (or Team 1)
Environment 02 → Participant B (or Team 2)
...
Environment 40 → Participant ZZ (or Team 40)
```

### Step 3: Distribute Credentials

**Create Credential Sheet:**

```
BCTalent Escape Room Event - Participant Credentials

Name: [Participant Full Name]
Environment: BCTalent-EscapeRoom-[Number]
Username: escaperoom[Number]@yourdomain.com
Password: [Generated Password]
URL: https://businesscentral.dynamics.com/[tenant]?environment=BCTalent-EscapeRoom-[Number]

Instructions:
1. Navigate to the URL above
2. Sign in with username and password
3. Search for "Escape Room Venues"
4. Choose your venue and get started!

Support Contact: [Facilitator Email/Phone]
```

**Distribution Methods:**
- Email (secure)
- Event platform direct message
- Printed handout (secure disposal after)
- QR code with link + credentials

---

## Event Start: Getting Participants Started

### Welcome Briefing (5-10 minutes)

**Cover:**
1. **Event Overview**
   - Purpose and learning objectives
   - Available venue tracks
   - Expected duration (60-90 minutes)

2. **How It Works**
   - Venue → Rooms → Tasks hierarchy
   - Task validation (automatic vs. manual)
   - Hints and solutions (with point penalties)
   - Scoring system

3. **Access Instructions**
   - How to log in
   - Finding Escape Room Venues
   - Entering name and partner info (IMPORTANT!)

4. **Support**
   - How to get help
   - Common issues
   - Facilitator contact info

5. **Leaderboard**
   - Real-time tracking
   - How points are scored
   - When results will be shared

### First Steps Guidance

**Walk participants through:**

1. **Log in to Business Central**
   - Use provided URL and credentials
   - Verify environment loads correctly

2. **Find Escape Room Venues**
   - Search bar: "Escape Room Venues"
   - Or navigate via Business Manager Role Center action

3. **Enter Participant Info**
   - **Full Name** - Exactly as you want on leaderboard
   - **Partner Name** - Company/team name
   - ⚠️ **CRITICAL:** This info identifies them in telemetry!

4. **Choose Venue**
   
5. **Open First Room**
   - Read room description carefully
   - Review all tasks
   - Start working!

---

## During Event: Active Support

### Monitoring Participant Progress

**Use Telemetry Dashboard:**
- See who's active vs. stuck
- Track completion rates
- Identify problematic tasks
- Monitor overall progress

**Key Metrics to Watch:**
- Tasks completed per minute
- Rooms started vs. completed
- Hint/solution usage
- Long gaps in activity (stuck participants)

### Providing Assistance

**Support Levels:**

**Level 1: Directional Hints (No Point Penalty)**
- General guidance without revealing solution
- Point to documentation/resources
- Clarify task requirements
- Answer questions about BC features

**Level 2: Task Hints (Built-in, -1 Point)**
- Participants request via "Show Hint" action
- Provides structured guidance
- Small point penalty encourages independent work

**Level 3: Detailed Help (Facilitator Discretion)**
- When participant truly stuck
- Time-sensitive situations
- Consider not penalizing if issue is environmental

**Level 4: Solution (Built-in, -3 Points)**
- Last resort after solution delay expires
- Via "Show Solution" action
- Significant point penalty

### Common Support Scenarios

**"I can't find the escape room feature"**
- Verify apps published and installed
- Check permission sets assigned
- Search for "Escape Room Venues"
- Check Role Center for action

**"Task won't validate even though I completed it"**
- Review validation criteria carefully
- Check for typos/case sensitivity
- Verify all sub-steps complete
- May need to refresh/reopen page
- Check Event Log for errors

**"I entered wrong name on venue"**
- Can't easily change after start
- Telemetry tracks original name
- Options: Live with it, or restart in new environment

**"My environment is slow/frozen"**
- Refresh browser
- Check network connectivity
- Verify sandbox status in Admin Center
- Move to backup environment if necessary

**"I accidentally opened wrong venue"**
- Can't undo once started
- Telemetry tracks all activity
- Consider switching environments
- Or continue in wrong venue (less ideal)

**"Solution button disabled"**
- Solution delay not expired yet (default 10 minutes)
- Shows remaining time
- Encourages continued effort

---

## Participant Best Practices

### Tips to Share

**Before Starting:**
- Read room descriptions fully
- Review ALL tasks before starting
- Plan your approach
- Don't rush!

**During Challenges:**
- Use BC documentation
- Check hints BEFORE getting stuck for too long
- Test your work incrementally
- Ask for help when truly stuck

**Maximizing Score:**
- Avoid hints when possible (-1 point each)
- Avoid solutions (-3 points each)
- Complete tasks quickly (no time bonus, but finishes faster)
- Complete entire venue for bonus points

**Learning Effectively:**
- Understand WHY, not just HOW
- Explore BC features beyond task requirements
- Collaborate with team (if team mode)
- Take notes for future reference

---

## Handling Edge Cases

### Late Arrivals

**Options:**
1. **Join in Progress** - Start where others are, won't win but can learn
2. **Separate Track** - Extend their time, separate leaderboard
3. **Team Placement** - Join existing team with availability

### Technical Issues

**Participant Can't Access:**
- Verify credentials correct
- Check environment status
- Verify security group assignment
- Check M365 license
- Move to backup environment

**App/Feature Not Working:**
- Check Event Log for errors
- Verify app fully installed
- Try republishing app to environment
- Use backup environment

**Lost Progress:**
- Telemetry preserved even if environment resets
- Can view completed tasks in telemetry
- May need to manually mark tasks complete
- Document for accurate leaderboard

### Disruptive Behavior

**If Participant:**
- Attempts to break environment
- Shares solutions with others
- Violates code of conduct

**Actions:**
- Private conversation first
- Remind of event guidelines
- Remove from environment if necessary
- Document incident

---

## Post-Event: Follow-Up

### Completion Verification

**Check Telemetry for:**
- Final scores
- Completion rates
- Time to complete
- Hint/solution usage

**Generate Final Leaderboard:**
- Overall winners
- Fastest completions
- Category winners (by venue)
- Perfect scores (no hints/solutions)

### Credential Cleanup

**Security Steps:**
1. **Disable user accounts** (or set expiration)
2. **Remove security group assignments**
3. **Delete or archive environments**
4. **Revoke any temporary licenses**
5. **Clear password records securely**

### Feedback Collection

**Survey Topics:**
- Event organization
- Challenge difficulty
- Learning value
- Technical issues
- Suggestions for improvement

---

## Accessibility Considerations

### For Participants with Disabilities

**Visual Impairments:**
- BC screen reader compatible
- High contrast themes available
- Zoom support
- May need extended time

**Hearing Impairments:**
- Provide written materials
- Use chat for support instead of voice
- Live transcription for briefings

**Motor Impairments:**
- Keyboard navigation supported
- Speech-to-text for typing
- Extended time allowed
- Team mode may help

**Cognitive Considerations:**
- Clear, simple instructions
- Allow breaks
- Extra time for complex tasks
- Facilitator check-ins

---

## Virtual vs. In-Person Events

### Virtual Event Specifics

**Advantages:**
- No travel required
- Record sessions for review
- Chat-based support efficient
- Screen sharing for debugging

**Challenges:**
- Technical issues harder to resolve
- Less participant engagement
- Difficult to monitor visually
- Time zone coordination

**Best Practices:**
- Pre-event tech check
- Virtual meeting room for support
- Screen share policy
- Recording consent

### In-Person Event Specifics

**Advantages:**
- Immediate face-to-face support
- Better engagement
- Collaborative atmosphere
- Easier to gauge progress visually

**Challenges:**
- Venue logistics
- Network capacity
- Physical space for facilitators
- Equipment needs

**Best Practices:**
- Reliable WiFi (test beforehand!)
- Power outlets for all
- Facilitators circulating
- Large display for leaderboard

---

## Related Documentation

- [Event Setup Guide](Event-Setup.md) - Environment preparation
- [Leaderboard Setup](Leaderboard-Setup.md) - Tracking and scoring
- [Framework Architecture](../Framework/Architecture.md) - How it works
- [Telemetry Integration](../Framework/Telemetry-Integration.md) - Event tracking

---

**Last Updated:** January 7, 2026
