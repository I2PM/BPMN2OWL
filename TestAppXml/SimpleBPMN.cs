using System;
using System.Collections.Generic;
namespace SimpleBPMN
{
	public class SimpleBPMN
	{
		public List<Participant> participants { get; set; }
		public List<MessageFlow> messageFlows { get; set; }
		public List<Process> processes { get; set; }
	}

	public class BPMNElement
	{
		public string id { get; set; }
		public string name { get; set; }
	}

	public class Participant : BPMNElement
	{
		public string processRef { get; set; }
	}

	public class MessageFlow : BPMNElement
	{
		public string sourceRef { get; set; }
		public Participant sourceParticipant { get; set; }
		public string targetRef { get; set; }
		public Participant targetParticipant { get; set; }
	}

	public class Process : BPMNElement
	{
		public Participant participant { get; set; }
		public StartEvent startEvent { get; set; }
		public List<Task> tasks { get; set; }
		public List<IntermediateThrowEvent> intermediateThrowEvents { get; set; }
		public List<IntermediateCatchMessageEvent> intermediateCatchMessageEvents { get; set; }
		public List<IntermediateCatchTimeEvent> intermediateCatchTimeEvents { get; set; }
		public List<EventBasedGateway> eventBasedGateways { get; set; }
		public List<EndEvent> endEvents { get; set; }
		public List<SequenceFlow> sequenceFlows { get; set; }
	}

	public class StartEvent : BPMNElement
	{
		public string outgoing { get; set; }
	}

	public class Task : BPMNElement
	{
		public List<string> incomings { get; set; }
		public List<string> outgoings { get; set; }
	}

	public class IntermediateThrowEvent : BPMNElement
	{
		public List<string> incomings { get; set; }
		public string outgoing { get; set; }
		public MessageFlow messageFlow { get; set; }
		public string incomingElementID { get; set; }
		public string outgoingElementID { get; set; }
	}

	public class IntermediateCatchMessageEvent : BPMNElement
	{
		public string incoming { get; set; }
		public string outgoing { get; set; }
		public MessageFlow messageFlow { get; set; }
		public string incomingElementID { get; set; }
		public string outgoingElementID { get; set; }
		public bool isStartEvent { get; set; } = false;
	}

	public class IntermediateCatchTimeEvent : BPMNElement
	{
		public string incoming { get; set; }
		public string outgoing { get; set; }
		public String timeDuration { get; set; }
	}

	public class EventBasedGateway : BPMNElement
	{
		public List<string> incomings { get; set; }
		public List<string> outgoings { get; set; }
		public string gatewayDirection { get; set; }
	}

	public class EndEvent : BPMNElement
	{
		public List<string> incomings { get; set; }
	}

	public class SequenceFlow : BPMNElement
	{
		public string sourceRef { get; set; }
		public string targetRef { get; set; }
	}
}