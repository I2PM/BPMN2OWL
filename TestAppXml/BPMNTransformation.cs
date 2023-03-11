using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAppXml
{
    class BPMNTransformation
    {
        public SimpleBPMN.SimpleBPMN BPMN2SimpleBPMN(BPMN.Definitions oldProcess)
        {
            SimpleBPMN.SimpleBPMN newProcess = new SimpleBPMN.SimpleBPMN();

            // Liste der Participants erstellen
            newProcess.participants = new List<SimpleBPMN.Participant>();
            // Liste Befüllen
            foreach (BPMN.Participant pOld in oldProcess.Collaboration.Participant)
            {
                SimpleBPMN.Participant pNew = new SimpleBPMN.Participant();
                pNew.name = pOld.Name;
                pNew.processRef = pOld.ProcessRef;
                pNew.id = pOld.Id;
                newProcess.participants.Add(pNew);
            }

            // Liste der messageFlows erstellen
            newProcess.messageFlows = new List<SimpleBPMN.MessageFlow>();
            // Liste Befüllen
            foreach (BPMN.MessageFlow mfOld in oldProcess.Collaboration.MessageFlow)
            {
                SimpleBPMN.MessageFlow mfNew = new SimpleBPMN.MessageFlow();
                mfNew.id = mfOld.Id;
                mfNew.name = mfOld.Name;
                mfNew.sourceRef = mfOld.SourceRef;
                mfNew.sourceParticipant = getParticipantFromElementId(mfOld.SourceRef, oldProcess, newProcess.participants);
                mfNew.targetRef = mfOld.TargetRef;
                mfNew.targetParticipant = getParticipantFromElementId(mfOld.TargetRef, oldProcess, newProcess.participants);
                newProcess.messageFlows.Add(mfNew);
            }

            // Liste der Processes erstellen
            newProcess.processes = new List<SimpleBPMN.Process>();
            // Liste Befüllen
            foreach (BPMN.Process pOld in oldProcess.Process)
            {
                SimpleBPMN.Process pNew = new SimpleBPMN.Process();
                pNew.id = pOld.Id;
                pNew.name = pOld.Name;
                pNew.participant = getParticipantFromProcess(pOld.Id, newProcess.participants);

                // Liste der intermediateCatchEvent für Message und Time erstellen
                pNew.intermediateCatchMessageEvents = new List<SimpleBPMN.IntermediateCatchMessageEvent>();

                //StartEvent erstellen
                if (pOld.StartEvent != null)
                {
                    if (pOld.StartEvent.MessageEventDefinition == null)
                    {
                        SimpleBPMN.StartEvent startNew = new SimpleBPMN.StartEvent();
                        startNew.id = pOld.StartEvent.Id;
                        startNew.name = pOld.StartEvent.Name;
                        startNew.outgoing = pOld.StartEvent.Outgoing;

                        pNew.startEvent = startNew;
                    }
                    else
                    {
                        SimpleBPMN.IntermediateCatchMessageEvent icmNew = new SimpleBPMN.IntermediateCatchMessageEvent();

                        icmNew.id = pOld.StartEvent.Id;
                        icmNew.name = pOld.StartEvent.Name;
                        //icmNew.incoming = icOld.Incoming;
                        icmNew.outgoing = pOld.StartEvent.Outgoing;
                        icmNew.messageFlow = getMessageFlowFromSourceOrTargetRef(pOld.StartEvent.Id, newProcess.messageFlows);
                        icmNew.isStartEvent = true;

                        pNew.intermediateCatchMessageEvents.Add(icmNew);
                    }

                    
                }

                // Liste der tasks erstellen
                pNew.tasks = new List<SimpleBPMN.Task>();
                // Liste Befüllen
                foreach (BPMN.Task tOld in pOld.Task)
                {
                    SimpleBPMN.Task tNew = new SimpleBPMN.Task();

                    tNew.id = tOld.Id;
                    tNew.name = tOld.Name;
                    tNew.incomings = tOld.Incoming;
                    tNew.outgoings = new List<string>();
                    tNew.outgoings.Add(tOld.Outgoing);

                    pNew.tasks.Add(tNew);
                }

                // Liste der intermediateThrowEvents erstellen
                pNew.intermediateThrowEvents = new List<SimpleBPMN.IntermediateThrowEvent>();
                // Liste Befüllen
                foreach (BPMN.IntermediateThrowEvent itOld in pOld.IntermediateThrowEvent)
                {
                    SimpleBPMN.IntermediateThrowEvent itNew = new SimpleBPMN.IntermediateThrowEvent();

                    itNew.id = itOld.Id;
                    itNew.name = itOld.Name;
                    itNew.incomings = itOld.Incoming;
                    itNew.outgoing = itOld.Outgoing;
                    itNew.messageFlow = getMessageFlowFromSourceOrTargetRef(itOld.Id, newProcess.messageFlows);

                    pNew.intermediateThrowEvents.Add(itNew);
                }

                // Liste der intermediateCatchEvent für Time erstellen
                pNew.intermediateCatchTimeEvents = new List<SimpleBPMN.IntermediateCatchTimeEvent>();
                // Liste Befüllen
                foreach (BPMN.IntermediateCatchEvent icOld in pOld.IntermediateCatchEvent)
                {
                    if (icOld.MessageEventDefinition != null)
                    {
                        SimpleBPMN.IntermediateCatchMessageEvent icmNew = new SimpleBPMN.IntermediateCatchMessageEvent();

                        icmNew.id = icOld.Id;
                        icmNew.name = icOld.Name;
                        icmNew.incoming = icOld.Incoming;
                        icmNew.outgoing = icOld.Outgoing;
                        icmNew.messageFlow = getMessageFlowFromSourceOrTargetRef(icOld.Id, newProcess.messageFlows);

                        pNew.intermediateCatchMessageEvents.Add(icmNew);
                    }
                    else
                    {
                        SimpleBPMN.IntermediateCatchTimeEvent ictNew = new SimpleBPMN.IntermediateCatchTimeEvent();

                        ictNew.id = icOld.Id;
                        ictNew.name = icOld.Name;
                        ictNew.incoming = icOld.Incoming;
                        ictNew.outgoing = icOld.Outgoing;
                        ictNew.timeDuration = icOld.TimerEventDefinition.TimeDuration.Text;

                        pNew.intermediateCatchTimeEvents.Add(ictNew);
                    }

                }

                // Liste der tasks mit exclusiveGateway Elementen Befüllen
                foreach (BPMN.ExclusiveGateway egOld in pOld.ExclusiveGateway)
                {
                    SimpleBPMN.Task tNew = new SimpleBPMN.Task();

                    tNew.id = egOld.Id;
                    tNew.name = egOld.Name;
                    tNew.incomings = egOld.Incoming;
                    tNew.outgoings = egOld.Outgoing;

                    pNew.tasks.Add(tNew);
                }

                // Liste der eventBasedGateways erstellen
                pNew.eventBasedGateways = new List<SimpleBPMN.EventBasedGateway>();
                // Liste Befüllen
                foreach (BPMN.EventBasedGateway ebgOld in pOld.EventBasedGateway)
                {
                    SimpleBPMN.EventBasedGateway ebgNew = new SimpleBPMN.EventBasedGateway();

                    ebgNew.id = ebgOld.Id;
                    ebgNew.name = ebgOld.Name;
                    ebgNew.incomings = ebgOld.Incoming;
                    ebgNew.outgoings = ebgOld.Outgoing;
                    ebgNew.gatewayDirection = ebgOld.GatewayDirection;

                    pNew.eventBasedGateways.Add(ebgNew);
                }

                // Liste der EndEvents erstellen
                pNew.endEvents = new List<SimpleBPMN.EndEvent>();
                // Liste Befüllen
                foreach (BPMN.EndEvent eeOld in pOld.EndEvent)
                {
                    SimpleBPMN.EndEvent eeNew = new SimpleBPMN.EndEvent();

                    eeNew.id = eeOld.Id;
                    eeNew.name = eeOld.Name;
                    eeNew.incomings = eeOld.Incoming;

                    pNew.endEvents.Add(eeNew);
                }

                // Liste der SequenceFlows erstellen
                pNew.sequenceFlows = new List<SimpleBPMN.SequenceFlow>();
                // Liste Befüllen
                foreach (BPMN.SequenceFlow sfOld in pOld.SequenceFlow)
                {
                    SimpleBPMN.SequenceFlow sfNew = new SimpleBPMN.SequenceFlow();

                    sfNew.id = sfOld.Id;
                    sfNew.name = sfOld.Name;
                    sfNew.sourceRef = sfOld.SourceRef;
                    sfNew.targetRef = sfOld.TargetRef;

                    pNew.sequenceFlows.Add(sfNew);
                }
                newProcess.processes.Add(pNew);
            }

            return newProcess;
        }

        public SimpleBPMN.Participant getParticipantFromProcess(String pId, List<SimpleBPMN.Participant> parts)
        {

            foreach (SimpleBPMN.Participant part in parts)
            {
                if (part.processRef == pId)
                {
                    return part;
                }
            }
            return null;
        }

        public SimpleBPMN.Participant getParticipantFromElementId(String eId, BPMN.Definitions bpmn, List<SimpleBPMN.Participant> newParts)
        {
            foreach (BPMN.Process proc in bpmn.Process)
            {
                foreach (string fnr in proc.LaneSet.Lane.FlowNodeRef)
                {
                    if (fnr == eId)
                    {
                        foreach (BPMN.Participant part in bpmn.Collaboration.Participant)
                            if (part.ProcessRef == proc.Id)
                            {
                                foreach (SimpleBPMN.Participant newPart in newParts)
                                {
                                    if (part.Id == newPart.id)
                                    {
                                        return newPart;
                                    }

                                }

                            }
                    }
                }
            }
            return null;
        }

        public SimpleBPMN.MessageFlow getMessageFlowFromSourceOrTargetRef(String id, List<SimpleBPMN.MessageFlow> mFlows)
        {
            foreach (SimpleBPMN.MessageFlow mFlow in mFlows)
            {
                if (mFlow.sourceRef == id)
                {
                    return mFlow;
                }
                if (mFlow.targetRef == id)
                {
                    return mFlow;
                }
            }
            return null;
        }

    }
}
