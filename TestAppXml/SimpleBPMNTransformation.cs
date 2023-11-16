using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAppXml
{
    class SimpleBPMNTransformation
    {
        public PASS.PASSElements SimpleBPMN2PASS(SimpleBPMN.SimpleBPMN oldProcess, string processName)
        {
            // neuen SimpleSBPM Process erstellen
            PASS.PASSElements newProcess = new PASS.PASSElements();

            // neues PASSProcessModel erstellen
            PASS.PASSProcessModel passProcessModel = new PASS.PASSProcessModel();

            passProcessModel.componentId = processName;
            passProcessModel.componentLabel = processName;

            newProcess.passProcessModel = passProcessModel;

            // SID erstellen
            PASS.ModelLayer sid = new PASS.ModelLayer();
            sid.componentId = "SID_1";
            sid.componentLabel = "SID_1";
            newProcess.modelLayer = sid;

            // MessageFlow Umwandeln
            newProcess.messageExchangeLists = new List<PASS.MessageExchangeList>();
            // Liste Befüllen
            foreach (SimpleBPMN.MessageFlow mfOld in oldProcess.messageFlows)
            {
                // MessageSpecification erstellen
                PASS.MessageSpecification msNew = new PASS.MessageSpecification();
                msNew.componentId = "MsgSpec_" + mfOld.id;
                msNew.componentLabel = mfOld.name;


                // MessageExchange erstellen
                PASS.MessageExchange meNew = new PASS.MessageExchange();
                meNew.bpmnId = mfOld.id;
                meNew.componentId = "SID_1_StdMsgConn_" + mfOld.sourceParticipant.id + "+" + mfOld.targetParticipant.id + "_MsgSpec_" + mfOld.id;
                meNew.componentLabel = "Message: " + mfOld.name + " From: " + mfOld.sourceParticipant.name + " To: " + mfOld.targetParticipant.name;
                meNew.sender = "SID_1_FullySpecifiedSubject_" + mfOld.sourceParticipant.id;
                meNew.receiver = "SID_1_FullySpecifiedSubject_" + mfOld.targetParticipant.id;
                meNew.messageSpecification = msNew;


                // MessageExchangeList erstellen
                string melNewComponentId = "MessageExchangeList_on_SID_1_StdMsgConn_" + mfOld.sourceParticipant.id + "+" + mfOld.targetParticipant.id;
                PASS.MessageExchangeList melNew = findMessageExchangeList(melNewComponentId, newProcess.messageExchangeLists);
                newProcess.messageExchangeLists.Remove(melNew);
                if (melNew == null)
                {
                    melNew = new PASS.MessageExchangeList();
                    melNew.componentId = melNewComponentId;
                    melNew.componentLabel = "SID_1_StdMsgConn_" + mfOld.sourceParticipant.id + "+" + mfOld.targetParticipant.id;

                    melNew.messageExchanges = new List<PASS.MessageExchange>();
                }

                melNew.messageExchanges.Add(meNew);
                newProcess.messageExchangeLists.Add(melNew);
            }


            // Liste der Subjects erstellen
            newProcess.subjects = new List<PASS.FullySpecifiedSubject>();
            // Liste befüllen
            foreach (SimpleBPMN.Participant pOld in oldProcess.participants)
            {
                // Subject erstellen
                PASS.FullySpecifiedSubject sNew = new PASS.FullySpecifiedSubject();
                sNew.bpmnId = pOld.id;
                sNew.componentId = "SID_1_FullySpecifiedSubject_" + pOld.id;
                sNew.componentLabel = pOld.name;

                sNew.subjectBehavior = "SBD_" + pOld.processRef + "_SID_1_FullySpecifiedSubject_" + pOld.id;

                newProcess.subjects.Add(sNew);                
            }

            // Liste der SubjectBehaviors erstellen
            newProcess.subjectBehaviors = new List<PASS.SubjectBehavior>();
            // Liste befüllen
            foreach (SimpleBPMN.Process pOld in oldProcess.processes)
            {
                // SubjectBehavior erstellen
                PASS.SubjectBehavior sbNew = new PASS.SubjectBehavior();

                sbNew.componentId = "SBD_" + pOld.id + "_SID_1_FullySpecifiedSubject_" + pOld.participant.id;
                sbNew.componentLabel = "SBD: " + pOld.participant.name;

                sbNew.subjectComponentId = "SID_1_FullySpecifiedSubject_" + pOld.participant.id;

                // Liste der Actions erstellen
                sbNew.actions = new List<PASS.Action>();
                // Liste befüllen

                // StartEvent umwandeln

                if (pOld.startEvent != null)
                {
                    PASS.DoState dssNew = new PASS.DoState();

                    dssNew.componentId = "SBD_" + pOld.id + "_DoState_" + pOld.startEvent.id;
                    dssNew.componentLabel = pOld.startEvent.name;

                    dssNew.isStartState = true;

                    PASS.Action asNew = new PASS.Action();

                    asNew.bpmnId = pOld.startEvent.id;
                    asNew.componentId = "action_of_SBD_" + pOld.id + "_DoState_" + pOld.startEvent.id;
                    asNew.componentLabel = "action_of_SBD_" + pOld.id + "_DoState_" + pOld.startEvent.id;

                    asNew.state = dssNew;
                    asNew.transitions = new List<PASS.Transition>();

                    sbNew.actions.Add(asNew);
                }

                

                // tasks umwandeln
                foreach (SimpleBPMN.Task tOld in pOld.tasks)
                {
                    // DoState erstellen
                    PASS.DoState dsNew = new PASS.DoState();

                    dsNew.componentId = "SBD_" + pOld.id + "_DoState_" + tOld.id;
                    dsNew.componentLabel = tOld.name;

                    PASS.Action aNew = new PASS.Action();

                    aNew.bpmnId = tOld.id;
                    aNew.componentId = "action_of_SBD_" + pOld.id + "_DoState_" + tOld.id;
                    aNew.componentLabel = "action_of_SBD_" + pOld.id + "_DoState_" + tOld.id;

                    aNew.state = dsNew;
                    aNew.transitions = new List<PASS.Transition>();

                    sbNew.actions.Add(aNew);
                }

                // ereignisbasiertes Gateway umwandeln
                foreach (SimpleBPMN.EventBasedGateway ebgOld in pOld.eventBasedGateways)
                {
                    // ReceiveState erstellen
                    PASS.ReceiveState rsNew = new PASS.ReceiveState();

                    rsNew.componentId = "SBD_" + pOld.id + "_ReceiveState_" + ebgOld.id;
                    rsNew.componentLabel = ebgOld.name;

                    PASS.Action aNew = new PASS.Action();

                    aNew.bpmnId = ebgOld.id;
                    aNew.componentId = "action_of_SBD_" + pOld.id + "_ReceiveState_" + ebgOld.id;
                    aNew.componentLabel = "action_of_SBD_" + pOld.id + "_ReceiveState_" + ebgOld.id;

                    aNew.state = rsNew;
                    aNew.transitions = new List<PASS.Transition>();

                    sbNew.actions.Add(aNew);
                }

                // endEvents umwandeln
                foreach (SimpleBPMN.EndEvent eeOld in pOld.endEvents)
                {
                    // DoState erstellen
                    PASS.DoState dsNew = new PASS.DoState();

                    dsNew.componentId = "SBD_" + pOld.id + "_DoState_" + eeOld.id;
                    dsNew.componentLabel = eeOld.name;

                    dsNew.isEndState = true;

                    PASS.Action aNew = new PASS.Action();

                    aNew.bpmnId = eeOld.id;
                    aNew.componentId = "action_of_SBD_" + pOld.id + "_DoState_" + eeOld.id;
                    aNew.componentLabel = "action_of_SBD_" + pOld.id + "_DoState_" + eeOld.id;

                    aNew.state = dsNew;

                    sbNew.actions.Add(aNew);
                }

                // imtermediateCatchTimeEvents umwandeln (State)
                foreach (SimpleBPMN.IntermediateCatchTimeEvent icetOld in pOld.intermediateCatchTimeEvents)
                {
                    if (!EventGatewayWasBefore(icetOld.incoming, pOld.sequenceFlows, pOld.eventBasedGateways))
                    {
                        // DoState erstellen
                        PASS.DoState dsNew = new PASS.DoState();

                        dsNew.componentId = "SBD_" + pOld.id + "_DoState_" + icetOld.id;
                        dsNew.componentLabel = icetOld.name;
                        dsNew.timeState = true;

                        PASS.Action aNew = new PASS.Action();

                        aNew.bpmnId = icetOld.id;
                        aNew.componentId = "action_of_SBD_" + pOld.id + "_DoState_" + icetOld.id;
                        aNew.componentLabel = "action_of_SBD_" + pOld.id + "_DoState_" + icetOld.id;

                        aNew.state = dsNew;
                        aNew.transitions = new List<PASS.Transition>();

                        sbNew.actions.Add(aNew);
                    } 
                }

                // imtermediateCatchMessageEvents umwandeln (State)
                foreach (SimpleBPMN.IntermediateCatchMessageEvent icemOld in pOld.intermediateCatchMessageEvents)
                {
                    if (!EventGatewayWasBefore(icemOld.incoming, pOld.sequenceFlows, pOld.eventBasedGateways))
                    {
                        // ReceiveState erstellen
                        PASS.ReceiveState rsNew = new PASS.ReceiveState();

                        rsNew.componentId = "SBD_" + pOld.id + "_ReceiveState_" + icemOld.id;
                        rsNew.componentLabel = icemOld.name;
                        rsNew.isStartState = icemOld.isStartEvent;

                        PASS.Action aNew = new PASS.Action();

                        aNew.bpmnId = icemOld.id;
                        aNew.componentId = "action_of_SBD_" + pOld.id + "_ReceiveState_" + icemOld.id;
                        aNew.componentLabel = "action_of_SBD_" + pOld.id + "_ReceiveState_" + icemOld.id;

                        aNew.state = rsNew;
                        aNew.transitions = new List<PASS.Transition>();

                        sbNew.actions.Add(aNew);
                    }
                }

                // ausloesende Zwischenereignisse umwandeln (State)
                foreach (SimpleBPMN.IntermediateThrowEvent iteOld in pOld.intermediateThrowEvents)
                {
                    // SendState erstellen
                    PASS.SendState ssNew = new PASS.SendState();

                    ssNew.componentId = "SBD_" + pOld.id + "_SendState_" + iteOld.id;
                    ssNew.componentLabel = iteOld.name;

                    // Action erstellen
                    PASS.Action aNew = new PASS.Action();

                    aNew.bpmnId = iteOld.id;
                    aNew.componentId = "action_of_SBD_" + pOld.id + "_SendState_" + iteOld.id;
                    aNew.componentLabel = "action_of_SBD_" + pOld.id + "_SendState_" + iteOld.id;

                    aNew.state = ssNew;
                    aNew.transitions = new List<PASS.Transition>();

                    sbNew.actions.Add(aNew);
                }

                // ausloesende Zwischenereignisse umwandeln (Transition)
                foreach (SimpleBPMN.IntermediateThrowEvent iteOld in pOld.intermediateThrowEvents)
                {
                    // SendTransitionCondition erstellen
                    PASS.SendTransitionCondition stcNew = new PASS.SendTransitionCondition();

                    stcNew.componentId = "SBD_" + pOld.id + "_SendTransition_" + iteOld.outgoing + "_sendTransitionCondition";
                    stcNew.componentLabel = iteOld.name;

                    PASS.MessageExchange me = findMessageExchange(iteOld.messageFlow.id, newProcess.messageExchangeLists);

                    stcNew.performedMessageExchange = me;
                    stcNew.messagSentTo = me.receiver;

                    // SendTransition erstellen erstellen
                    PASS.SendTransition stNew = new PASS.SendTransition();

                    stNew.componentId = "SBD_" + pOld.id + "_SendTransition_" + iteOld.outgoing;
                    stNew.componentLabel = "To: " + iteOld.messageFlow.targetParticipant.name + " Msg: " + iteOld.messageFlow.name;

                    string targetId = FindBPMNIdOfTargetState(iteOld.outgoing, pOld.sequenceFlows);

                    stNew.sourceState = FindActionWithId(iteOld.id, sbNew.actions).state;
                    stNew.targetState = FindActionWithId(targetId, sbNew.actions).state;
                    stNew.transitionCondition = stcNew;

                    // Action finden
                    PASS.Action aNew;

                    aNew = FindActionWithId(iteOld.id, sbNew.actions);

                    sbNew.actions.Remove(aNew);
                    aNew.transitions.Add(stNew);
                    sbNew.actions.Add(aNew);
                }


                // imtermediateCatchTimeEvents umwandeln (Transition)
                foreach (SimpleBPMN.IntermediateCatchTimeEvent icetOld in pOld.intermediateCatchTimeEvents)
                {
                    // DayTimeTimerTransitionCondition erstellen erstellen
                    PASS.DayTimeTimerTransitionCondition ttcNew = new PASS.DayTimeTimerTransitionCondition();

                    ttcNew.componentId = "SBD_" + pOld.id + "_DayTimeTimerTransition_" + icetOld.outgoing + "_DayTimeTimerTransitionCondition";                    
                    ttcNew.componentLabel = icetOld.name;

                    ttcNew.duration = icetOld.timeDuration;

                    // DayTimeTimerTransition erstellen erstellen
                    PASS.DayTimeTimerTransition ttNew = new PASS.DayTimeTimerTransition();

                    
                    ttNew.componentLabel = icetOld.name;

                    if (EventGatewayWasBefore(icetOld.incoming, pOld.sequenceFlows, pOld.eventBasedGateways))
                    {
                        ttNew.componentId = "SBD_" + pOld.id + "_DayTimeTimerTransition_" + icetOld.incoming + "+" + icetOld.id + "+" + icetOld.outgoing;

                        string sourceId = FindBPMNIdOfSourceState(icetOld.incoming, pOld.sequenceFlows);
                        ttNew.sourceState = FindActionWithId(sourceId, sbNew.actions).state;
                    }
                    else
                    {
                        ttNew.componentId = "SBD_" + pOld.id + "_DayTimeTimerTransition_" + icetOld.outgoing;

                        ttNew.sourceState = FindActionWithId(icetOld.id, sbNew.actions).state;
                    }

                    string targetId = FindBPMNIdOfTargetState(icetOld.outgoing, pOld.sequenceFlows);

                    ttNew.targetState = FindActionWithId(targetId, sbNew.actions).state;
                    ttNew.transitionCondition = ttcNew;

                    // Action finden
                    PASS.Action aNew;

                    if (EventGatewayWasBefore(icetOld.incoming, pOld.sequenceFlows, pOld.eventBasedGateways))
                    {
                        string sourceId = FindBPMNIdOfSourceState(icetOld.incoming, pOld.sequenceFlows);
                        aNew = FindActionWithId(sourceId, sbNew.actions);
                    }
                    else
                    {
                        aNew = FindActionWithId(icetOld.id, sbNew.actions);
                    }

                    sbNew.actions.Remove(aNew);

                    aNew.transitions.Add(ttNew);

                    sbNew.actions.Add(aNew);
                }

                // imtermediateCatchMessageEvents umwandeln (Transition)
                foreach (SimpleBPMN.IntermediateCatchMessageEvent icemOld in pOld.intermediateCatchMessageEvents)
                {
                    // ReceiveTransitionCondition erstellen
                    PASS.ReceiveTransitionCondition rtcNew = new PASS.ReceiveTransitionCondition();
                    
                    rtcNew.componentId = "SBD_" + pOld.id + "_ReceiveTransition_" + icemOld.outgoing + "_receiveTransitionCondition";
                    rtcNew.componentLabel = icemOld.name;

                    PASS.MessageExchange me = findMessageExchange(icemOld.messageFlow.id, newProcess.messageExchangeLists);

                    rtcNew.performedMessageExchange = me;
                    rtcNew.messagSentFrom = me.sender;

                    // ReceiveTransition erstellen erstellen
                    PASS.ReceiveTransition rtNew = new PASS.ReceiveTransition();

                    
                    rtNew.componentLabel = "From: " + icemOld.messageFlow.sourceParticipant.name + " Msg: " + icemOld.messageFlow.name;

                    if (EventGatewayWasBefore(icemOld.incoming, pOld.sequenceFlows, pOld.eventBasedGateways))
                    {
                        rtNew.componentId = "SBD_" + pOld.id + "_ReceiveTransition_" + icemOld.incoming + "+" + icemOld.id + "+" + icemOld.outgoing;

                        string sourceId = FindBPMNIdOfSourceState(icemOld.incoming, pOld.sequenceFlows);
                        rtNew.sourceState = FindActionWithId(sourceId, sbNew.actions).state;
                    }
                    else
                    {
                        rtNew.componentId = "SBD_" + pOld.id + "_ReceiveTransition_" + icemOld.outgoing;

                        rtNew.sourceState = FindActionWithId(icemOld.id, sbNew.actions).state;
                    }

                    string targetId = FindBPMNIdOfTargetState(icemOld.outgoing, pOld.sequenceFlows);
                    rtNew.targetState = FindActionWithId(targetId, sbNew.actions).state;
                    rtNew.transitionCondition = rtcNew;

                    // Action finden
                    PASS.Action aNew;

                    if (EventGatewayWasBefore(icemOld.incoming, pOld.sequenceFlows, pOld.eventBasedGateways))
                    {
                        string sourceId = FindBPMNIdOfSourceState(icemOld.incoming, pOld.sequenceFlows);
                        aNew = FindActionWithId(sourceId, sbNew.actions);
                    }
                    else
                    {
                        aNew = FindActionWithId(icemOld.id, sbNew.actions);
                    }

                    sbNew.actions.Remove(aNew);

                    aNew.transitions.Add(rtNew);

                    sbNew.actions.Add(aNew);
                }

                // sequenceFlows umwandeln
                foreach (SimpleBPMN.SequenceFlow sfOld in pOld.sequenceFlows)
                {
                    if (isDoState(sfOld.sourceRef, sbNew.actions))
                    {
                        // DoTransition erstellen
                        PASS.DoTransition rtNew = new PASS.DoTransition();

                        rtNew.componentId = "SBD_" + pOld.id + "_DoTransition_" + sfOld.id;
                        rtNew.componentLabel = sfOld.name;

                        rtNew.sourceState = FindActionWithId(sfOld.sourceRef, sbNew.actions).state;
                        rtNew.targetState = FindActionWithId(sfOld.targetRef, sbNew.actions).state;

                        // Action finden
                        PASS.Action aNew = FindActionWithId(sfOld.sourceRef, sbNew.actions);
                        sbNew.actions.Remove(aNew);

                        aNew.transitions.Add(rtNew);

                        sbNew.actions.Add(aNew);
                    } 
                }
                newProcess.subjectBehaviors.Add(sbNew);
            }

            // PASSProcessModel verfolständigen
            newProcess.passProcessModel.contains = new List<string>();
            newProcess.passProcessModel.contains.Add(newProcess.modelLayer.componentId);

            //ModelLayer verfolständigen
            newProcess.modelLayer.contains = new List<string>();

            foreach (PASS.FullySpecifiedSubject subj in newProcess.subjects)
            {
                newProcess.passProcessModel.contains.Add(subj.componentId);
                newProcess.modelLayer.contains.Add(subj.componentId);
            }

            foreach (PASS.SubjectBehavior subjb in newProcess.subjectBehaviors)
            {
                newProcess.passProcessModel.contains.Add(subjb.componentId);
                newProcess.modelLayer.contains.Add(subjb.componentId);
            }

            foreach (PASS.MessageExchangeList mel in newProcess.messageExchangeLists)
            {
                newProcess.passProcessModel.contains.Add(mel.componentId);
                newProcess.modelLayer.contains.Add(mel.componentId);
                foreach (PASS.MessageExchange me in mel.messageExchanges)
                {
                    newProcess.passProcessModel.contains.Add(me.componentId);
                    newProcess.modelLayer.contains.Add(me.componentId);

                    newProcess.passProcessModel.contains.Add(me.messageSpecification.componentId);
                    newProcess.modelLayer.contains.Add(me.messageSpecification.componentId);
                }
            }

            return newProcess;
        }

        public BPMN.Definitions SimpleBPMN2BPMN(SimpleBPMN.SimpleBPMN oldProcess)
        {
            BPMN.Definitions newProcess = new BPMN.Definitions();

            // Collaboration erstellen
            newProcess.Collaboration = new BPMN.Collaboration();

            // Liste der Participants erstellen
            newProcess.Collaboration.Participant = new List<BPMN.Participant>();

            // Liste Befüllen
            foreach (SimpleBPMN.Participant pOld in oldProcess.participants)
            {
                BPMN.Participant pNew = new BPMN.Participant();
                pNew.Name = pOld.name;
                pNew.ProcessRef = pOld.processRef;
                pNew.Id = pOld.id;

                newProcess.Collaboration.Participant.Add(pNew);
            }

            // Liste der messageFlows erstellen
            newProcess.Collaboration.MessageFlow = new List<BPMN.MessageFlow>();

            // Liste Befüllen
            foreach (SimpleBPMN.MessageFlow mfOld in oldProcess.messageFlows)
            {
                BPMN.MessageFlow mfNew = new BPMN.MessageFlow();
                mfNew.Id = mfOld.id;
                mfNew.Name = mfOld.name;
                mfNew.SourceRef = mfOld.sourceRef;
                mfNew.TargetRef = mfOld.targetRef;

                newProcess.Collaboration.MessageFlow.Add(mfNew);
            }

            // Liste der Processes erstellen
            newProcess.Process = new List<BPMN.Process>();
            
            // Liste Befüllen
            foreach (SimpleBPMN.Process pOld in oldProcess.processes)
            {
                
                BPMN.Process pNew = new BPMN.Process();
                pNew.Id = pOld.id;
                pNew.Name = pOld.name;

                // LaneSet + Lane erstellen

                pNew.LaneSet = new BPMN.LaneSet();
                pNew.LaneSet.Lane = new BPMN.Lane();
                pNew.LaneSet.Lane.FlowNodeRef = new List<string>();

                //StartEvent erstellen
                if (pOld.startEvent != null)
                {
                    BPMN.StartEvent startNew = new BPMN.StartEvent();
                    startNew.Id = pOld.startEvent.id;
                    startNew.Name = pOld.startEvent.name;
                    startNew.Outgoing = pOld.startEvent.outgoing;

                    pNew.StartEvent = startNew;
                    pNew.LaneSet.Lane.FlowNodeRef.Add(startNew.Id);
                }
                

                // Liste der tasks and exclusive Gateways erstellen
                pNew.Task = new List<BPMN.Task>();
                pNew.ExclusiveGateway = new List<BPMN.ExclusiveGateway>();

                // Liste Befüllen
                foreach (SimpleBPMN.Task tOld in pOld.tasks)
                {
                    // Task
                    if (tOld.outgoings.Count == 1)
                    {
                        BPMN.Task tNew = new BPMN.Task();

                        tNew.Id = tOld.id;
                        tNew.Name = tOld.name;
                        tNew.Incoming = tOld.incomings;
                        tNew.Outgoing = tOld.outgoings[0];

                        pNew.Task.Add(tNew);
                        pNew.LaneSet.Lane.FlowNodeRef.Add(tNew.Id);
                    }
                    // Exclusive Gateway
                    else if (tOld.outgoings.Count > 1)
                    {
                        BPMN.ExclusiveGateway egNew = new BPMN.ExclusiveGateway();

                        egNew.Id = tOld.id;
                        egNew.Name = tOld.name;
                        egNew.Incoming = tOld.incomings;
                        egNew.Outgoing = tOld.outgoings;

                        pNew.ExclusiveGateway.Add(egNew);
                        pNew.LaneSet.Lane.FlowNodeRef.Add(egNew.Id);
                    }
                }
                
                // Liste der intermediateThrowEvents erstellen
                pNew.IntermediateThrowEvent = new List<BPMN.IntermediateThrowEvent>();

                // Liste Befüllen
                foreach (SimpleBPMN.IntermediateThrowEvent itOld in pOld.intermediateThrowEvents)
                {
                    BPMN.IntermediateThrowEvent itNew = new BPMN.IntermediateThrowEvent();

                    itNew.Id = itOld.id;
                    itNew.Name = itOld.name;
                    itNew.Incoming = itOld.incomings;
                    itNew.Outgoing = itOld.outgoing;
                    itNew.MessageEventDefinition = new BPMN.MessageEventDefinition();

                    pNew.IntermediateThrowEvent.Add(itNew);
                    pNew.LaneSet.Lane.FlowNodeRef.Add(itNew.Id);
                }
                
                // Liste der intermediateCatchEvents erstellen
                pNew.IntermediateCatchEvent = new List<BPMN.IntermediateCatchEvent>();

                // Liste Befüllen
                foreach (SimpleBPMN.IntermediateCatchMessageEvent icmOld in pOld.intermediateCatchMessageEvents)
                {
                    if (icmOld.isStartEvent)
                    {
                        BPMN.StartEvent startNew = new BPMN.StartEvent();
                        startNew.Id = icmOld.id;
                        startNew.Name = icmOld.name;
                        startNew.Outgoing = icmOld.outgoing;
                        startNew.MessageEventDefinition = new BPMN.MessageEventDefinition();

                        pNew.StartEvent = startNew;
                        pNew.LaneSet.Lane.FlowNodeRef.Add(startNew.Id);
                    }
                    else
                    {
                        BPMN.IntermediateCatchEvent icmNew = new BPMN.IntermediateCatchEvent();

                        icmNew.Id = icmOld.id;
                        icmNew.Name = icmOld.name;
                        icmNew.Incoming = icmOld.incoming;
                        icmNew.Outgoing = icmOld.outgoing;
                        icmNew.MessageEventDefinition = new BPMN.MessageEventDefinition();

                        pNew.IntermediateCatchEvent.Add(icmNew);
                        pNew.LaneSet.Lane.FlowNodeRef.Add(icmNew.Id);
                    }    
                }

                foreach (SimpleBPMN.IntermediateCatchTimeEvent ictOld in pOld.intermediateCatchTimeEvents)
                {
                    BPMN.IntermediateCatchEvent ictNew = new BPMN.IntermediateCatchEvent();

                    ictNew.Id = ictOld.id;
                    ictNew.Name = ictOld.name;
                    ictNew.Incoming = ictOld.incoming;
                    ictNew.Outgoing = ictOld.outgoing;
                    ictNew.TimerEventDefinition = new BPMN.TimerEventDefinition();
                    ictNew.TimerEventDefinition.TimeDuration = new BPMN.TimeDuration();
                    ictNew.TimerEventDefinition.TimeDuration.Text = ictOld.timeDuration;

                    pNew.IntermediateCatchEvent.Add(ictNew);
                    pNew.LaneSet.Lane.FlowNodeRef.Add(ictNew.Id);
                }

                // Liste der eventBasedGateways erstellen
                pNew.EventBasedGateway = new List<BPMN.EventBasedGateway>();

                // Liste Befüllen
                foreach (SimpleBPMN.EventBasedGateway ebgOld in pOld.eventBasedGateways)
                {
                    BPMN.EventBasedGateway ebgNew = new BPMN.EventBasedGateway();

                    ebgNew.Id = ebgOld.id;
                    ebgNew.Name = ebgOld.name;
                    ebgNew.Incoming = ebgOld.incomings;
                    ebgNew.Outgoing = ebgOld.outgoings;

                    pNew.EventBasedGateway.Add(ebgNew);
                    pNew.LaneSet.Lane.FlowNodeRef.Add(ebgNew.Id);
                }
                
                // Liste der EndEvents erstellen
                pNew.EndEvent = new List<BPMN.EndEvent>();

                // Liste Befüllen
                foreach (SimpleBPMN.EndEvent eeOld in pOld.endEvents)
                {
                    BPMN.EndEvent eeNew = new BPMN.EndEvent();

                    eeNew.Id = eeOld.id;
                    eeNew.Name = eeOld.name;
                    eeNew.Incoming = eeOld.incomings;

                    pNew.EndEvent.Add(eeNew);
                    pNew.LaneSet.Lane.FlowNodeRef.Add(eeNew.Id);
                }
                
                // Liste der SequenceFlows erstellen
                pNew.SequenceFlow = new List<BPMN.SequenceFlow>();

                // Liste Befüllen
                foreach (SimpleBPMN.SequenceFlow sfOld in pOld.sequenceFlows)
                {
                    BPMN.SequenceFlow sfNew = new BPMN.SequenceFlow();

                    sfNew.Id = sfOld.id;
                    sfNew.Name = sfOld.name;
                    sfNew.SourceRef = sfOld.sourceRef;
                    sfNew.TargetRef = sfOld.targetRef;

                    pNew.SequenceFlow.Add(sfNew);
                }
                

                newProcess.Process.Add(pNew);
            }

            return newProcess;
        }

        public bool isDoState(string bpmnId, List<PASS.Action> list)
        {
            foreach (PASS.Action act in list)
            {
                if (act.state is PASS.DoState && act.bpmnId == bpmnId && !act.state.timeState)
                {
                    return true;
                }
            }
            return false;
        }

        public PASS.MessageExchange findMessageExchange(string bpmnId, List<PASS.MessageExchangeList> list)
        {
            foreach (PASS.MessageExchangeList mel in list)
            {
                foreach (PASS.MessageExchange me in mel.messageExchanges)
                {
                    if (me.bpmnId == bpmnId)
                    {
                        return me;
                    }
                }
            }
            return null;
        }

        public PASS.MessageExchangeList findMessageExchangeList(string componentId, List<PASS.MessageExchangeList> list)
        {
            foreach (PASS.MessageExchangeList mel in list)
            {
                if (mel.componentId == componentId)
                {
                    return mel;
                }
            }
            return null;
        }

        public string FindBPMNIdOfSourceState(string incomingId, List<SimpleBPMN.SequenceFlow> list)
        {
            foreach (SimpleBPMN.SequenceFlow sf in list)
            {
                if (sf.id == incomingId)
                {
                    return sf.sourceRef;
                }
            }
            return null;
        }

        public bool EventGatewayWasBefore (string incomingId, List<SimpleBPMN.SequenceFlow> sequenceList, List<SimpleBPMN.EventBasedGateway> gwList)
        {
            foreach (SimpleBPMN.SequenceFlow sf in sequenceList)
            {
                if (sf.id == incomingId)
                {
                    foreach (SimpleBPMN.EventBasedGateway gw in gwList)
                    {
                        if (sf.sourceRef == gw.id)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public string FindBPMNIdOfTargetState(string outgoingId, List<SimpleBPMN.SequenceFlow> list)
        {
            foreach (SimpleBPMN.SequenceFlow sf in list)
            {
                if (sf.id == outgoingId)
                {
                    return sf.targetRef;
                }
            }
            return null;
        }

        public PASS.Action FindActionWithId(string id, List<PASS.Action> list)
        {
            foreach (PASS.Action act in list)
            {
                if (act.bpmnId == id)
                {
                    return act;
                }
            }
            return null;
        }
    }
}
