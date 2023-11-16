/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace BPMN
{

	[XmlRoot(ElementName = "participantMultiplicity")]
	public class ParticipantMultiplicity
	{
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }
		[XmlAttribute(AttributeName = "maximum")]
		public string Maximum { get; set; }
		[XmlAttribute(AttributeName = "minimum")]
		public string Minimum { get; set; }
	}

	[XmlRoot(ElementName = "participant", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
	public class Participant : IComparable<Participant>
	{
		[XmlElement(ElementName = "participantMultiplicity")]
		public ParticipantMultiplicity ParticipantMultiplicity { get; set; }
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }
		[XmlAttribute(AttributeName = "processRef")]
		public string ProcessRef { get; set; }

		// Default comparer for Participant type.
		public int CompareTo(Participant compareParticipant)
		{
			// A null value means that this object is greater.
			if (compareParticipant == null)
				return 1;

			else
				return this.Id.CompareTo(compareParticipant.Id);
		}

		public bool IsTheSame(Participant otherParticipant)
		{
			return Id == otherParticipant.Id &&
				Name == otherParticipant.Name &&
				ProcessRef == otherParticipant.ProcessRef;
		}
	}

	[XmlRoot(ElementName = "messageFlow", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
	public class MessageFlow : IComparable<MessageFlow>
	{
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }
		[XmlAttribute(AttributeName = "sourceRef")]
		public string SourceRef { get; set; }
		[XmlAttribute(AttributeName = "targetRef")]
		public string TargetRef { get; set; }

		// Default comparer for Participant type.
		public int CompareTo(MessageFlow compareMessageFlow)
		{
			// A null value means that this object is greater.
			if (compareMessageFlow == null)
				return 1;

			else
				return this.Id.CompareTo(compareMessageFlow.Id);
		}

		public bool IsTheSame(MessageFlow otherMessageFlow)
		{
			return Id == otherMessageFlow.Id &&
				Name == otherMessageFlow.Name &&
				SourceRef == otherMessageFlow.SourceRef &&
				TargetRef == otherMessageFlow.TargetRef;
		}
	}

	[XmlRoot(ElementName = "collaboration", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
	public class Collaboration
	{
		[XmlElement(ElementName = "participant", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public List<Participant> Participant { get; set; }
		[XmlElement(ElementName = "messageFlow", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public List<MessageFlow> MessageFlow { get; set; }
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }

		public bool IsTheSame(Collaboration otherCollaboration)
		{
			List<Participant> listp1 = Participant;
			List<Participant> listp2 = otherCollaboration.Participant;
			listp1.Sort();
			listp2.Sort();

			int n = Participant.Count;
			bool same = true;

			for (int i = 0; i < n; i++)
			{
				same = same && listp1[i].IsTheSame(listp2[i]);
			}

			List<MessageFlow> listmf1 = MessageFlow;
			List<MessageFlow> listmf2 = otherCollaboration.MessageFlow;
			listmf1.Sort();
			listmf2.Sort();

			n = MessageFlow.Count;

			for (int i = 0; i < n; i++)
			{
				same = same && listmf1[i].IsTheSame(listmf2[i]);
			}

			return same;
		}
	}

	[XmlRoot(ElementName = "lane", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
	public class Lane
	{
		[XmlElement(ElementName = "flowNodeRef", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public List<string> FlowNodeRef { get; set; }
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }

		public bool IsTheSame(Lane otherLane)
		{
			bool same = true;

			// Compare FlowNodeRef
			List<string> list1 = FlowNodeRef;
			List<string> list2 = otherLane.FlowNodeRef;
			list1.Sort();
			list2.Sort();

			int n = FlowNodeRef.Count;

			for (int i = 0; i < n; i++)
			{
				same = same && list1[i] == list2[i];
			}

			return same;
		}
	}

	[XmlRoot(ElementName = "laneSet", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
	public class LaneSet
	{
		[XmlElement(ElementName = "lane", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public Lane Lane { get; set; }
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }

		public bool IsTheSame(LaneSet otherLaneSet)
		{
			bool same = Lane.IsTheSame(otherLaneSet.Lane);

			return same;
		}
	}

	[XmlRoot(ElementName = "startEvent", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
	public class StartEvent
	{
		[XmlElement(ElementName = "outgoing", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public string Outgoing { get; set; }
		[XmlElement(ElementName = "messageEventDefinition", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public MessageEventDefinition MessageEventDefinition { get; set; }
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }

		public bool IsTheSame(StartEvent otherStartEvent)
		{
			return Id == otherStartEvent.Id &&
				Name == otherStartEvent.Name &&
				Outgoing == otherStartEvent.Outgoing;
		}
	}

	[XmlRoot(ElementName = "task", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
	public class Task : IComparable<Task>
	{
		[XmlElement(ElementName = "incoming", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public List<string> Incoming { get; set; }
		[XmlElement(ElementName = "outgoing", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public string Outgoing { get; set; }
		[XmlAttribute(AttributeName = "completionQuantity")]
		public string CompletionQuantity { get; set; }
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }
		[XmlAttribute(AttributeName = "isForCompensation")]
		public string IsForCompensation { get; set; }
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }
		[XmlAttribute(AttributeName = "startQuantity")]
		public string StartQuantity { get; set; }

		public int CompareTo(Task compareTask)
		{
			// A null value means that this object is greater.
			if (compareTask == null)
				return 1;

			else
				return this.Id.CompareTo(compareTask.Id);
		}

		public bool IsTheSame(Task otherTask)
		{
			bool same = Id == otherTask.Id &&
				Name == otherTask.Name &&
				Outgoing == otherTask.Outgoing;

			// Compare Incoming
			List<string> list1 = Incoming;
			List<string> list2 = otherTask.Incoming;
			list1.Sort();
			list2.Sort();

			int n = Incoming.Count;

			for (int i = 0; i < n; i++)
			{
				same = same && list1[i] == list2[i];
			}

			return same;
		}
	}

	[XmlRoot(ElementName = "exclusiveGateway", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
	public class ExclusiveGateway : IComparable<ExclusiveGateway>
	{
		[XmlElement(ElementName = "incoming", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public List<string> Incoming { get; set; }
		[XmlElement(ElementName = "outgoing", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public List<string> Outgoing { get; set; }
		[XmlAttribute(AttributeName = "gatewayDirection")]
		public string GatewayDirection { get; set; }
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }

		public int CompareTo(ExclusiveGateway compareExclusiveGateway)
		{
			// A null value means that this object is greater.
			if (compareExclusiveGateway == null)
				return 1;

			else
				return this.Id.CompareTo(compareExclusiveGateway.Id);
		}

		public bool IsTheSame(ExclusiveGateway otherExclusiveGateway)
		{
			bool same = Id == otherExclusiveGateway.Id &&
				Name == otherExclusiveGateway.Name;

			// Compare Incoming
			List<string> listI1 = Incoming;
			List<string> listI2 = otherExclusiveGateway.Incoming;
			listI1.Sort();
			listI2.Sort();

			int n = Incoming.Count;

			for (int i = 0; i < n; i++)
			{
				same = same && listI1[i] == listI2[i];
			}

			// Compare Outgoing
			List<string> listO1 = Outgoing;
			List<string> listO2 = otherExclusiveGateway.Outgoing;
			listO1.Sort();
			listO2.Sort();

			n = Outgoing.Count;

			for (int i = 0; i < n; i++)
			{
				same = same && listO1[i] == listO2[i];
			}

			return same;
		}
	}

	[XmlRoot(ElementName = "messageEventDefinition", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
	public class MessageEventDefinition
	{
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }
	}

	[XmlRoot(ElementName = "intermediateThrowEvent", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
	public class IntermediateThrowEvent : IComparable<IntermediateThrowEvent>
	{
		[XmlElement(ElementName = "incoming", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public List<string> Incoming { get; set; }
		[XmlElement(ElementName = "outgoing", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public string Outgoing { get; set; }
		[XmlElement(ElementName = "messageEventDefinition", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public MessageEventDefinition MessageEventDefinition { get; set; }
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }

		public int CompareTo(IntermediateThrowEvent compareIntermediateThrowEvent)
		{
			// A null value means that this object is greater.
			if (compareIntermediateThrowEvent == null)
				return 1;

			else
				return this.Id.CompareTo(compareIntermediateThrowEvent.Id);
		}
		public bool IsTheSame(IntermediateThrowEvent otherIntermediateThrowEvent)
		{
			bool same = Id == otherIntermediateThrowEvent.Id &&
				Name == otherIntermediateThrowEvent.Name &&
				Outgoing == otherIntermediateThrowEvent.Outgoing;

			// Compare Incoming
			List<string> list1 = Incoming;
			List<string> list2 = otherIntermediateThrowEvent.Incoming;
			list1.Sort();
			list2.Sort();

			int n = Incoming.Count;

			for (int i = 0; i < n; i++)
			{
				same = same && list1[i] == list2[i];
			}

			return same;
		}
	}

	[XmlRoot(ElementName = "endEvent", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
	public class EndEvent : IComparable<EndEvent>
	{
		[XmlElement(ElementName = "incoming", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public List<string> Incoming { get; set; }
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }

		public int CompareTo(EndEvent compareEndEvent)
		{
			// A null value means that this object is greater.
			if (compareEndEvent == null)
				return 1;

			else
				return this.Id.CompareTo(compareEndEvent.Id);
		}

		public bool IsTheSame(EndEvent otherEndEvent)
		{
			bool same = Id == otherEndEvent.Id &&
				Name == otherEndEvent.Name;

			// Compare Incoming
			List<string> list1 = Incoming;
			List<string> list2 = otherEndEvent.Incoming;
			list1.Sort();
			list2.Sort();

			int n = Incoming.Count;

			for (int i = 0; i < n; i++)
			{
				same = same && list1[i] == list2[i];
			}

			return same;
		}
	}

	[XmlRoot(ElementName = "sequenceFlow", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
	public class SequenceFlow : IComparable<SequenceFlow>
	{
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }
		[XmlAttribute(AttributeName = "sourceRef")]
		public string SourceRef { get; set; }
		[XmlAttribute(AttributeName = "targetRef")]
		public string TargetRef { get; set; }

		public int CompareTo(SequenceFlow compareSequenceFlow)
		{
			// A null value means that this object is greater.
			if (compareSequenceFlow == null)
				return 1;

			else
				return this.Id.CompareTo(compareSequenceFlow.Id);
		}

		public bool IsTheSame(SequenceFlow otherSequenceFlow)
		{
			return Id == otherSequenceFlow.Id &&
				SourceRef == otherSequenceFlow.SourceRef &&
				TargetRef == otherSequenceFlow.TargetRef;
		}
	}

	[XmlRoot(ElementName = "process", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
	public class Process : IComparable<Process>
	{
		[XmlElement(ElementName = "laneSet", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public LaneSet LaneSet { get; set; }
		[XmlElement(ElementName = "startEvent", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public StartEvent StartEvent { get; set; }
		[XmlElement(ElementName = "task", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public List<Task> Task { get; set; }
		[XmlElement(ElementName = "exclusiveGateway", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public List<ExclusiveGateway> ExclusiveGateway { get; set; }
		[XmlElement(ElementName = "intermediateThrowEvent", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public List<IntermediateThrowEvent> IntermediateThrowEvent { get; set; }
		[XmlElement(ElementName = "endEvent", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public List<EndEvent> EndEvent { get; set; }
		[XmlElement(ElementName = "sequenceFlow", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public List<SequenceFlow> SequenceFlow { get; set; }
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }
		[XmlAttribute(AttributeName = "isClosed")]
		public string IsClosed { get; set; }
		[XmlAttribute(AttributeName = "isExecutable")]
		public string IsExecutable { get; set; }
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }
		[XmlAttribute(AttributeName = "processType")]
		public string ProcessType { get; set; }
		[XmlElement(ElementName = "intermediateCatchEvent", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public List<IntermediateCatchEvent> IntermediateCatchEvent { get; set; }
		[XmlElement(ElementName = "eventBasedGateway", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public List<EventBasedGateway> EventBasedGateway { get; set; }

		// Default comparer for Participant type.
		public int CompareTo(Process compareProcess)
		{
			// A null value means that this object is greater.
			if (compareProcess == null)
				return 1;

			else
				return this.Id.CompareTo(compareProcess.Id);
		}

		public bool IsTheSame(Process otherProcess)
		{
			bool same = Id == otherProcess.Id;
			same = same && Name == otherProcess.Name;

			// Compare LaneSet
			same = same && LaneSet.IsTheSame(otherProcess.LaneSet);

			// Compare StartEvent
			same = same && StartEvent.IsTheSame(otherProcess.StartEvent);

			// Compare Tasks
			List<Task> listt1 = Task;
			List<Task> listt2 = otherProcess.Task;
			listt1.Sort();
			listt2.Sort();

			int n = Task.Count;

			for (int i = 0; i < n; i++)
			{
				same = same && listt1[i].IsTheSame(listt2[i]);
			}

			// Compare Exclusive Gateways
			List<ExclusiveGateway> listeg1 = ExclusiveGateway;
			List<ExclusiveGateway> listeg2 = otherProcess.ExclusiveGateway;
			listeg1.Sort();
			listeg2.Sort();

			n = ExclusiveGateway.Count;

			for (int i = 0; i < n; i++)
			{
				same = same && listeg1[i].IsTheSame(listeg2[i]);
			}

			// Compare IntermediateThrowEvent
			List<IntermediateThrowEvent> listite1 = IntermediateThrowEvent;
			List<IntermediateThrowEvent> listite2 = otherProcess.IntermediateThrowEvent;
			listite1.Sort();
			listite2.Sort();

			n = IntermediateThrowEvent.Count;

			for (int i = 0; i < n; i++)
			{
				same = same && listite1[i].IsTheSame(listite2[i]);
			}

			// Compare EndEvent
			List<EndEvent> listee1 = EndEvent;
			List<EndEvent> listee2 = otherProcess.EndEvent;
			listee1.Sort();
			listee2.Sort();

			n = EndEvent.Count;

			for (int i = 0; i < n; i++)
			{
				same = same && listee1[i].IsTheSame(listee2[i]);
			}

			// Compare SequenceFlow
			List<SequenceFlow> listsf1 = SequenceFlow;
			List<SequenceFlow> listsf2 = otherProcess.SequenceFlow;
			listsf1.Sort();
			listsf2.Sort();

			n = SequenceFlow.Count;

			for (int i = 0; i < n; i++)
			{
				same = same && listsf1[i].IsTheSame(listsf2[i]);
			}

			// Compare IntermediateCatchEvent
			List<IntermediateCatchEvent> listice1 = IntermediateCatchEvent;
			List<IntermediateCatchEvent> listice2 = otherProcess.IntermediateCatchEvent;
			listice1.Sort();
			listice2.Sort();

			n = IntermediateCatchEvent.Count;

			for (int i = 0; i < n; i++)
			{
				same = same && listice1[i].IsTheSame(listice2[i]);
			}

			// Compare EventBasedGateway
			List<EventBasedGateway> listebg1 = EventBasedGateway;
			List<EventBasedGateway> listebg2 = otherProcess.EventBasedGateway;
			listebg1.Sort();
			listebg2.Sort();

			n = EventBasedGateway.Count;

			for (int i = 0; i < n; i++)
			{
				same = same && listebg1[i].IsTheSame(listebg2[i]);
			}

			return same;
		}

	}

	[XmlRoot(ElementName = "intermediateCatchEvent", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
	public class IntermediateCatchEvent : IComparable<IntermediateCatchEvent>
	{
		[XmlElement(ElementName = "incoming", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public string Incoming { get; set; }
		[XmlElement(ElementName = "outgoing", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public string Outgoing { get; set; }
		[XmlElement(ElementName = "messageEventDefinition", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public MessageEventDefinition MessageEventDefinition { get; set; }
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }
		[XmlElement(ElementName = "timerEventDefinition", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public TimerEventDefinition TimerEventDefinition { get; set; }

		public int CompareTo(IntermediateCatchEvent compareIntermediateCatchEvent)
		{
			// A null value means that this object is greater.
			if (compareIntermediateCatchEvent == null)
				return 1;

			else
				return this.Id.CompareTo(compareIntermediateCatchEvent.Id);
		}

		public bool IsTheSame(IntermediateCatchEvent otherIntermediateCatchEvent)
		{
			bool same = Id == otherIntermediateCatchEvent.Id &&
				Name == otherIntermediateCatchEvent.Name &&
				Incoming == otherIntermediateCatchEvent.Incoming &&
				Outgoing == otherIntermediateCatchEvent.Outgoing;

			if (TimerEventDefinition != null)
			{
				same = same && TimerEventDefinition.TimeDuration.Text == otherIntermediateCatchEvent.TimerEventDefinition.TimeDuration.Text;
			}

			return same;
		}
	}

	[XmlRoot(ElementName = "timeDuration")]//, Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
	public class TimeDuration
	{
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "timerEventDefinition", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
	public class TimerEventDefinition
	{
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }
		[XmlElement(ElementName = "timeCycle", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public TimeCycle TimeCycle { get; set; }
		[XmlElement(ElementName = "timeDuration", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public TimeDuration TimeDuration { get; set; }
	}

	[XmlRoot(ElementName = "eventBasedGateway", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
	public class EventBasedGateway : IComparable<EventBasedGateway>
	{
		[XmlElement(ElementName = "incoming", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public List<string> Incoming { get; set; }
		[XmlElement(ElementName = "outgoing", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public List<string> Outgoing { get; set; }
		[XmlAttribute(AttributeName = "eventGatewayType")]
		public string EventGatewayType { get; set; }
		[XmlAttribute(AttributeName = "gatewayDirection")]
		public string GatewayDirection { get; set; }
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }
		[XmlAttribute(AttributeName = "instantiate")]
		public string Instantiate { get; set; }
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }

		public int CompareTo(EventBasedGateway compareEventBasedGateway)
		{
			// A null value means that this object is greater.
			if (compareEventBasedGateway == null)
				return 1;

			else
				return this.Id.CompareTo(compareEventBasedGateway.Id);
		}

		public bool IsTheSame(EventBasedGateway otherEventBasedGateway)
		{
			bool same = Id == otherEventBasedGateway.Id;

			// Compare Incoming
			List<string> listI1 = Incoming;
			List<string> listI2 = otherEventBasedGateway.Incoming;
			listI1.Sort();
			listI2.Sort();

			int n = Incoming.Count;

			for (int i = 0; i < n; i++)
			{
				same = same && listI1[i] == listI2[i];
			}

			// Compare Outgoing
			List<string> listO1 = Outgoing;
			List<string> listO2 = otherEventBasedGateway.Outgoing;
			listO1.Sort();
			listO2.Sort();

			n = Outgoing.Count;

			for (int i = 0; i < n; i++)
			{
				same = same && listO1[i] == listO2[i];
			}

			return same;
		}
	}

	[XmlRoot(ElementName = "timeCycle", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
	public class TimeCycle
	{
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "Bounds", Namespace = "http://www.omg.org/spec/DD/20100524/DC")]
	public class Bounds
	{
		[XmlAttribute(AttributeName = "height")]
		public string Height { get; set; }
		[XmlAttribute(AttributeName = "width")]
		public string Width { get; set; }
		[XmlAttribute(AttributeName = "x")]
		public string X { get; set; }
		[XmlAttribute(AttributeName = "y")]
		public string Y { get; set; }
	}

	[XmlRoot(ElementName = "BPMNLabel", Namespace = "http://www.omg.org/spec/BPMN/20100524/DI")]
	public class BPMNLabel
	{
		[XmlElement(ElementName = "Bounds", Namespace = "http://www.omg.org/spec/DD/20100524/DC")]
		public Bounds Bounds { get; set; }
		[XmlAttribute(AttributeName = "labelStyle")]
		public string LabelStyle { get; set; }
	}

	[XmlRoot(ElementName = "BPMNShape", Namespace = "http://www.omg.org/spec/BPMN/20100524/DI")]
	public class BPMNShape
	{
		[XmlElement(ElementName = "Bounds", Namespace = "http://www.omg.org/spec/DD/20100524/DC")]
		public Bounds Bounds { get; set; }
		[XmlElement(ElementName = "BPMNLabel", Namespace = "http://www.omg.org/spec/BPMN/20100524/DI")]
		public BPMNLabel BPMNLabel { get; set; }
		[XmlAttribute(AttributeName = "bpmnElement")]
		public string BpmnElement { get; set; }
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }
		[XmlAttribute(AttributeName = "isHorizontal")]
		public string IsHorizontal { get; set; }
		[XmlAttribute(AttributeName = "isMarkerVisible")]
		public string IsMarkerVisible { get; set; }
	}

	[XmlRoot(ElementName = "waypoint", Namespace = "http://www.omg.org/spec/DD/20100524/DI")]
	public class Waypoint
	{
		[XmlAttribute(AttributeName = "x")]
		public string X { get; set; }
		[XmlAttribute(AttributeName = "y")]
		public string Y { get; set; }
	}

	[XmlRoot(ElementName = "BPMNEdge", Namespace = "http://www.omg.org/spec/BPMN/20100524/DI")]
	public class BPMNEdge
	{
		[XmlElement(ElementName = "waypoint", Namespace = "http://www.omg.org/spec/DD/20100524/DI")]
		public List<Waypoint> Waypoint { get; set; }
		[XmlAttribute(AttributeName = "bpmnElement")]
		public string BpmnElement { get; set; }
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }
		[XmlElement(ElementName = "BPMNLabel", Namespace = "http://www.omg.org/spec/BPMN/20100524/DI")]
		public BPMNLabel BPMNLabel { get; set; }
	}

	[XmlRoot(ElementName = "BPMNPlane", Namespace = "http://www.omg.org/spec/BPMN/20100524/DI")]
	public class BPMNPlane
	{
		[XmlElement(ElementName = "BPMNShape", Namespace = "http://www.omg.org/spec/BPMN/20100524/DI")]
		public List<BPMNShape> BPMNShape { get; set; }
		[XmlElement(ElementName = "BPMNEdge", Namespace = "http://www.omg.org/spec/BPMN/20100524/DI")]
		public List<BPMNEdge> BPMNEdge { get; set; }
		[XmlAttribute(AttributeName = "bpmnElement")]
		public string BpmnElement { get; set; }
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }
	}

	[XmlRoot(ElementName = "Font", Namespace = "http://www.omg.org/spec/DD/20100524/DC")]
	public class Font
	{
		[XmlAttribute(AttributeName = "isBold")]
		public string IsBold { get; set; }
		[XmlAttribute(AttributeName = "isItalic")]
		public string IsItalic { get; set; }
		[XmlAttribute(AttributeName = "isStrikeThrough")]
		public string IsStrikeThrough { get; set; }
		[XmlAttribute(AttributeName = "isUnderline")]
		public string IsUnderline { get; set; }
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }
		[XmlAttribute(AttributeName = "size")]
		public string Size { get; set; }
	}

	[XmlRoot(ElementName = "BPMNLabelStyle", Namespace = "http://www.omg.org/spec/BPMN/20100524/DI")]
	public class BPMNLabelStyle
	{
		[XmlElement(ElementName = "Font", Namespace = "http://www.omg.org/spec/DD/20100524/DC")]
		public Font Font { get; set; }
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }
	}

	[XmlRoot(ElementName = "BPMNDiagram", Namespace = "http://www.omg.org/spec/BPMN/20100524/DI")]
	public class BPMNDiagram
	{
		[XmlElement(ElementName = "BPMNPlane", Namespace = "http://www.omg.org/spec/BPMN/20100524/DI")]
		public BPMNPlane BPMNPlane { get; set; }
		[XmlElement(ElementName = "BPMNLabelStyle", Namespace = "http://www.omg.org/spec/BPMN/20100524/DI")]
		public List<BPMNLabelStyle> BPMNLabelStyle { get; set; }
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }
	}

	[XmlRoot(ElementName = "definitions", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
	public class Definitions
	{
		[XmlElement(ElementName = "collaboration", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public Collaboration Collaboration { get; set; }
		[XmlElement(ElementName = "process", Namespace = "http://www.omg.org/spec/BPMN/20100524/MODEL")]
		public List<Process> Process { get; set; }
		[XmlElement(ElementName = "BPMNDiagram", Namespace = "http://www.omg.org/spec/BPMN/20100524/DI")]
		public BPMNDiagram BPMNDiagram { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
		[XmlAttribute(AttributeName = "bpmndi", Namespace = "http://www.w3.org/2000/xmlns/")]
		public string Bpmndi { get; set; }
		[XmlAttribute(AttributeName = "omgdc", Namespace = "http://www.w3.org/2000/xmlns/")]
		public string Omgdc { get; set; }
		[XmlAttribute(AttributeName = "omgdi", Namespace = "http://www.w3.org/2000/xmlns/")]
		public string Omgdi { get; set; }
		[XmlAttribute(AttributeName = "signavio", Namespace = "http://www.w3.org/2000/xmlns/")]
		public string Signavio { get; set; }
		[XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
		public string Xsi { get; set; }
		[XmlAttribute(AttributeName = "exporter")]
		public string Exporter { get; set; }
		[XmlAttribute(AttributeName = "exporterVersion")]
		public string ExporterVersion { get; set; }
		[XmlAttribute(AttributeName = "expressionLanguage")]
		public string ExpressionLanguage { get; set; }
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }
		[XmlAttribute(AttributeName = "targetNamespace")]
		public string TargetNamespace { get; set; }
		[XmlAttribute(AttributeName = "typeLanguage")]
		public string TypeLanguage { get; set; }
		[XmlAttribute(AttributeName = "schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
		public string SchemaLocation { get; set; }

		public bool IsTheSame(Definitions otherModel)
        {
			// Compare Collaboration
			bool same = Collaboration.IsTheSame(otherModel.Collaboration);

			// Compare Processes
			List<Process> listp1 = Process;
			List<Process> listp2 = otherModel.Process;
			listp1.Sort();
			listp2.Sort();

			int n = Process.Count;

			for (int i = 0; i < n; i++)
			{
				same = same && listp1[i].IsTheSame(listp2[i]);
			}

			return same;
		}
	}

}