using System.Collections.Generic;
using alps.net.api.ALPS;
using alps.net.api.StandardPASS;

namespace TestAppXml
{
    class PASSTransformation
    {
        public IPASSProcessModel PASS2ProcessModel(PASS.PASSElements oldProcess)
        {
            // ################################################### export ####################################################################

            // Create Model
            IPASSProcessModel model = new PASSProcessModel("http://subjective-me.jimdo.com/s-bpm/processmodels/2022-05-18/tim/", oldProcess.passProcessModel.componentLabel);
            model.setModelComponentID(oldProcess.passProcessModel.componentId);

            // Create SID
            IModelLayer layer = new ModelLayer(model, oldProcess.modelLayer.componentLabel);
            layer.setModelComponentID(oldProcess.modelLayer.componentId);

            // The layer is already registered in the model (because the model was passed in the constructor),
            // but the layer is currently not the base layer
            model.setBaseLayer(layer);

            // Create Subjects with Base Behavior
            
            foreach (PASS.FullySpecifiedSubject oldSubj in oldProcess.subjects)
            {
                ISubjectBaseBehavior subjBe = new SubjectBaseBehavior(layer, oldSubj.subjectBehavior);
                IFullySpecifiedSubject subj = new FullySpecifiedSubject(layer, oldSubj.componentLabel, null, subjBe);
                subj.setModelComponentID(oldSubj.componentId);
                if (IsStartSubject(oldProcess.subjectBehaviors, oldSubj.subjectBehavior))
                {
                    subj.assignRole(ISubject.Role.StartSubject);
                    model.addStartSubject(subj);
                }
                

                //todo Multi Subject
            }

            // Create Message Exchange
            List<IMessageExchange> messageExchangeList = new List<IMessageExchange>();

            foreach (PASS.MessageExchangeList oldMel in oldProcess.messageExchangeLists)
            {
                IMessageExchangeList newMel = new MessageExchangeList(layer, oldMel.componentLabel);
                newMel.setModelComponentID(oldMel.componentId);

                foreach (PASS.MessageExchange oldMe in oldMel.messageExchanges)
                {
                    IMessageSpecification newMSpec = new MessageSpecification(layer, oldMe.messageSpecification.componentLabel);
                    newMSpec.setModelComponentID(oldMe.messageSpecification.componentId);

                    // find sender and receiver in layer
                    int senderIndex = oldProcess.subjects.FindIndex(a => a.componentId == oldMe.sender);
                    int receiverIndex = oldProcess.subjects.FindIndex(a => a.componentId == oldMe.receiver);
                    IFullySpecifiedSubject sender = layer.getFullySpecifiedSubject(senderIndex);
                    IFullySpecifiedSubject receiver = layer.getFullySpecifiedSubject(receiverIndex);

                    IMessageExchange newMe = new MessageExchange(layer, oldMe.componentLabel, newMSpec, sender, receiver);
                    newMe.setModelComponentID(oldMe.componentId);

                    messageExchangeList.Add(newMe);
                    newMel.addContainsMessageExchange(newMe);
                }
            }

            foreach (PASS.SubjectBehavior oldSubjBe in oldProcess.subjectBehaviors)
            {
                // Find the Behavior
                int senderIndex = oldProcess.subjects.FindIndex(a => a.componentId == oldSubjBe.subjectComponentId);
                IFullySpecifiedSubject newSubj = layer.getFullySpecifiedSubject(senderIndex);
                ISubjectBehavior newSubjBe = newSubj.getSubjectBaseBehavior();
                newSubjBe.setModelComponentID(oldSubjBe.componentId);

                List<IState> stateList = new List<IState>();

                foreach(PASS.Action oldAction in oldSubjBe.actions)
                {
                    if (oldAction.state is PASS.DoState)
                    {
                        if (oldAction.state.componentLabel == "")
                        {
                            oldAction.state.componentLabel = "Standard DoState";
                        }

                        IDoState doState = new DoState(newSubjBe, oldAction.state.componentLabel);
                        doState.setModelComponentID(oldAction.state.componentId);

                        IAction action = doState.getAction();
                        action.setModelComponentID(oldAction.componentId);

                        if (oldAction.state.isStartState)
                        {
                            newSubjBe.setInitialState(doState);
                        }

                        if (oldAction.state.isEndState)
                        {
                            doState.setIsStateType(IState.StateType.EndState);
                        }
                        stateList.Add(doState);
                    }

                    if (oldAction.state is PASS.SendState)
                    {
                        if (oldAction.state.componentLabel == "")
                        {
                            oldAction.state.componentLabel = "Standard SendState";
                        }

                        ISendState sendState = new SendState(newSubjBe, oldAction.state.componentLabel);
                        sendState.setModelComponentID(oldAction.state.componentId);

                        IAction action = sendState.getAction();
                        action.setModelComponentID(oldAction.componentId);

                        if (oldAction.state.isStartState)
                        {
                            newSubjBe.setInitialState(sendState);
                        }

                        if (oldAction.state.isEndState)
                        {
                            sendState.setIsStateType(IState.StateType.EndState);
                        }
                        stateList.Add(sendState);
                    }

                    if (oldAction.state is PASS.ReceiveState)
                    {
                        if (oldAction.state.componentLabel == "")
                        {
                            oldAction.state.componentLabel = "Standard ReceiveState";
                        }

                        IReceiveState receiveState = new ReceiveState(newSubjBe, oldAction.state.componentLabel);
                        receiveState.setModelComponentID(oldAction.state.componentId);

                        IAction action = receiveState.getAction();
                        action.setModelComponentID(oldAction.componentId);

                        if (oldAction.state.isStartState)
                        {
                            newSubjBe.setInitialState(receiveState);
                        }

                        if (oldAction.state.isEndState)
                        {
                            receiveState.setIsStateType(IState.StateType.EndState);
                        }
                        stateList.Add(receiveState);
                    }
                }

                foreach (PASS.Action oldAction in oldSubjBe.actions)
                {
                    if (oldAction.transitions != null)
                    {
                        foreach (PASS.Transition oldTrans in oldAction.transitions)
                        {
                            if (oldTrans is PASS.DoTransition)
                            {
                                IState sourceState = stateList.Find(a => a.getModelComponentID() == oldTrans.sourceState.componentId);
                                IState targetState = stateList.Find(a => a.getModelComponentID() == oldTrans.targetState.componentId);

                                if (oldTrans.componentLabel == "")
                                {
                                    oldTrans.componentLabel = "Standard DoTransition";
                                }
                                IDoTransition doTransition = new DoTransition(sourceState, targetState, oldTrans.componentLabel);
                                doTransition.setModelComponentID(oldTrans.componentId);
                            }

                            if (oldTrans is PASS.SendTransition)
                            {
                                IState sourceState = stateList.Find(a => a.getModelComponentID() == oldTrans.sourceState.componentId);
                                IState targetState = stateList.Find(a => a.getModelComponentID() == oldTrans.targetState.componentId);

                                if (oldTrans.componentLabel == "")
                                {
                                    oldTrans.componentLabel = "Standard SendTransition";
                                }

                                ISendTransition sendTransition = new SendTransition(sourceState, targetState, oldTrans.componentLabel);
                                sendTransition.setModelComponentID(oldTrans.componentId);

                                IMessageExchange messEx = messageExchangeList.Find(a => a.getModelComponentID() == oldTrans.transitionCondition.performedMessageExchange.componentId);
                                ISendTransitionCondition sendTranCon = new SendTransitionCondition(sendTransition, oldTrans.transitionCondition.componentLabel, null, messEx, 0, 0, null, messEx.getReceiver(), messEx.getMessageType());
                                sendTranCon.setModelComponentID(oldTrans.transitionCondition.componentId);
                            }

                            if (oldTrans is PASS.ReceiveTransition)
                            {
                                IState sourceState = stateList.Find(a => a.getModelComponentID() == oldTrans.sourceState.componentId);
                                IState targetState = stateList.Find(a => a.getModelComponentID() == oldTrans.targetState.componentId);

                                if (oldTrans.componentLabel == "")
                                {
                                    oldTrans.componentLabel = "Standard ReceiveTransition";
                                }

                                IReceiveTransition receiveTransition = new ReceiveTransition(sourceState, targetState, oldTrans.componentLabel);
                                receiveTransition.setModelComponentID(oldTrans.componentId);

                                IMessageExchange messEx = messageExchangeList.Find(a => a.getModelComponentID() == oldTrans.transitionCondition.performedMessageExchange.componentId);
                                IReceiveTransitionCondition receiveTranCon = new ReceiveTransitionCondition(receiveTransition, oldTrans.transitionCondition.componentLabel, null, messEx, 0, 0, null, messEx.getReceiver(), messEx.getMessageType());
                                receiveTranCon.setModelComponentID(oldTrans.transitionCondition.componentId);
                            }

                            if (oldTrans is PASS.DayTimeTimerTransition)
                            {
                                IState sourceState = stateList.Find(a => a.getModelComponentID() == oldTrans.sourceState.componentId);
                                IState targetState = stateList.Find(a => a.getModelComponentID() == oldTrans.targetState.componentId);

                                if (oldTrans.componentLabel == "")
                                {
                                    oldTrans.componentLabel = "Standard DayTimeTimerTransition";
                                }

                                IDayTimeTimerTransition timerTransition = new DayTimeTimerTransition(sourceState, targetState, oldTrans.componentLabel);
                                timerTransition.setModelComponentID(oldTrans.componentId);
                                
                                IDayTimeTimerTransitionCondition timerTranCon = new DayTimeTimerTransitionCondition(timerTransition, oldTrans.transitionCondition.componentLabel, null, oldTrans.transitionCondition.duration);
                                timerTranCon.setModelComponentID(oldTrans.transitionCondition.componentId);
                                timerTranCon.setTimeValue(oldTrans.transitionCondition.duration);
                            }
                        }
                    }
                }
            }

            //IDictionary<string, IPASSProcessModelElement> test = layer.getElements();

            return model;
        }


        public SimpleBPMN.SimpleBPMN PASS2SimpleBPMN(PASS.PASSElements oldProcess)
        {
            SimpleBPMN.SimpleBPMN newProcess = new SimpleBPMN.SimpleBPMN();

            //Participants erstellen
            newProcess.participants = new List<SimpleBPMN.Participant>();
     
            foreach (PASS.FullySpecifiedSubject subjOld in oldProcess.subjects)
            {
                SimpleBPMN.Participant pNew = new SimpleBPMN.Participant();
                pNew.name = subjOld.componentLabel;
                pNew.processRef = FindBPMNSid(subjOld.subjectBehavior,0);
                pNew.id = FindBPMNSid(subjOld.componentId, 0);
                newProcess.participants.Add(pNew);
            }

            // Liste der messageFlows erstellen
            newProcess.messageFlows = new List<SimpleBPMN.MessageFlow>();
            // Liste Befüllen
            foreach (PASS.MessageExchangeList melOld in oldProcess.messageExchangeLists)
            {
                foreach (PASS.MessageExchange meOld in melOld.messageExchanges)
                {
                    SimpleBPMN.MessageFlow mfNew = new SimpleBPMN.MessageFlow();
                    mfNew.id = FindBPMNSid(meOld.componentId, 2);
                    mfNew.name = meOld.messageSpecification.componentLabel;
                    mfNew.sourceRef = FindSourceRef(oldProcess.subjectBehaviors, meOld);
                    mfNew.targetRef = FindTargetRef(oldProcess.subjectBehaviors, meOld);
                    newProcess.messageFlows.Add(mfNew);
                }
            }

            // Liste der Processes erstellen
            newProcess.processes = new List<SimpleBPMN.Process>();
            // Liste Befüllen
            foreach (PASS.SubjectBehavior subBeOld in oldProcess.subjectBehaviors)
            {
                SimpleBPMN.Process pNew = new SimpleBPMN.Process();
                pNew.id = FindBPMNSid(subBeOld.componentId, 0);
                pNew.name = FindSubjectName(subBeOld.subjectComponentId, oldProcess.subjects);

                // Liste der EndEvents erstellen
                pNew.endEvents = new List<SimpleBPMN.EndEvent>();

                // Liste der tasks erstellen
                pNew.tasks = new List<SimpleBPMN.Task>();

                // Liste der intermediateThrowEvents erstellen
                pNew.intermediateThrowEvents = new List<SimpleBPMN.IntermediateThrowEvent>();

                // Liste der intermediateCatchEvent für Message und Time erstellen
                pNew.intermediateCatchMessageEvents = new List<SimpleBPMN.IntermediateCatchMessageEvent>();
                pNew.intermediateCatchTimeEvents = new List<SimpleBPMN.IntermediateCatchTimeEvent>();

                // Liste der eventBasedGateways erstellen
                pNew.eventBasedGateways = new List<SimpleBPMN.EventBasedGateway>();

                // Liste der SequenceFlows erstellen
                pNew.sequenceFlows = new List<SimpleBPMN.SequenceFlow>();

                foreach (PASS.Action act in subBeOld.actions)
                {
                    //StartEvent erstellen
                    if (act.state.isStartState && act.state is PASS.DoState)
                    {
                        SimpleBPMN.StartEvent startNew = new SimpleBPMN.StartEvent();
                        startNew.id = FindBPMNSid(act.componentId, 1);
                        startNew.name = act.state.componentLabel;
                        startNew.outgoing = FindBPMNSid(act.transitions[0].componentId, 1);

                        pNew.startEvent = startNew;
                    }
                    else if (act.state.isEndState)
                    {
                        SimpleBPMN.EndEvent eeNew = new SimpleBPMN.EndEvent();
                        eeNew.id = FindBPMNSid(act.componentId, 1);
                        eeNew.name = act.state.componentLabel;
                        eeNew.incomings = FindIncomings(act.state.componentId, subBeOld.actions);

                        pNew.endEvents.Add(eeNew);
                    }
                    else if (act.state is PASS.DoState && act.transitions[0] is PASS.DayTimeTimerTransition)
                    {
                        SimpleBPMN.IntermediateCatchTimeEvent ictNew = new SimpleBPMN.IntermediateCatchTimeEvent
                        {
                            id = FindBPMNSid(act.componentId, 1),
                            name = act.state.componentLabel,
                            incoming = FindIncomings(act.state.componentId, subBeOld.actions)[0],
                            outgoing = FindBPMNSid(act.transitions[0].componentId, 1),
                            timeDuration = act.transitions[0].transitionCondition.duration
                        };

                        pNew.intermediateCatchTimeEvents.Add(ictNew);
                    }
                    else if (act.state is PASS.DoState)
                    {
                        // Liste der Tasks Befüllen
                        SimpleBPMN.Task tNew = new SimpleBPMN.Task();

                        tNew.id = FindBPMNSid(act.componentId, 1);
                        tNew.name = act.state.componentLabel;
                        tNew.incomings = FindIncomings(act.state.componentId, subBeOld.actions);
                        
                        tNew.outgoings = new List<string>();
                        foreach (PASS.Transition tra in act.transitions)
                        {
                            tNew.outgoings.Add(FindBPMNSid(tra.componentId, 1));
                        }

                        pNew.tasks.Add(tNew);
                    }
                    else if (act.state is PASS.SendState)
                    {
                        // Liste der intermediateThrowEvents Befüllen
                        SimpleBPMN.IntermediateThrowEvent itNew = new SimpleBPMN.IntermediateThrowEvent
                        {
                            id = FindBPMNSid(act.componentId, 1),
                            name = act.transitions[0].transitionCondition.componentLabel,
                            incomings = FindIncomings(act.state.componentId, subBeOld.actions),
                            outgoing = FindBPMNSid(act.transitions[0].componentId, 1)
                        };

                        pNew.intermediateThrowEvents.Add(itNew);
                    }
                    else if (act.state is PASS.ReceiveState && act.transitions.Count == 1)
                    {
                        SimpleBPMN.IntermediateCatchMessageEvent icmNew = new SimpleBPMN.IntermediateCatchMessageEvent();
                        icmNew.id = FindBPMNSid(act.componentId, 1);
                        icmNew.name = act.transitions[0].transitionCondition.componentLabel;
                        List<string> incomings = FindIncomings(act.state.componentId, subBeOld.actions);
                        if (incomings.Count > 0)
                        {
                            icmNew.incoming = incomings[0];
                        }
                        icmNew.outgoing = FindBPMNSid(act.transitions[0].componentId, 1);
                        icmNew.isStartEvent = act.state.isStartState;
                        
                        pNew.intermediateCatchMessageEvents.Add(icmNew);
                    }
                    else if (act.state is PASS.ReceiveState)
                    {
                        SimpleBPMN.EventBasedGateway ebgNew = new SimpleBPMN.EventBasedGateway();

                        ebgNew.id = FindBPMNSid(act.componentId, 1);
                        ebgNew.name = act.state.componentLabel;
                        ebgNew.incomings = FindIncomings(act.state.componentId, subBeOld.actions);

                        ebgNew.outgoings = new List<string>();
                        foreach (PASS.Transition tra in act.transitions)
                        {
                            ebgNew.outgoings.Add(FindBPMNSid(tra.componentId, 1));
                        }

                        pNew.eventBasedGateways.Add(ebgNew);
                    }

                    if (act.transitions != null)
                    {
                        foreach (PASS.Transition tra in act.transitions)
                        {
                            // Event Based Gateway AND ReceiveTransition
                            if (act.transitions.Count > 1 && act.state is PASS.ReceiveState && tra is PASS.ReceiveTransition)
                            {
                                // sequence Flow before

                                SimpleBPMN.SequenceFlow sfNew1 = new SimpleBPMN.SequenceFlow();

                                sfNew1.id = FindBPMNSid(tra.componentId, 1);
                                sfNew1.name = "";
                                sfNew1.sourceRef = FindBPMNSid(tra.sourceState.componentId, 1);
                                sfNew1.targetRef = FindBPMNSid(tra.componentId, 2);

                                pNew.sequenceFlows.Add(sfNew1);

                                // incoming IntermediateCatchMessageEvent

                                SimpleBPMN.IntermediateCatchMessageEvent icmNew = new SimpleBPMN.IntermediateCatchMessageEvent
                                {
                                    id = FindBPMNSid(tra.componentId, 2),
                                    name = tra.transitionCondition.componentLabel,
                                    incoming = FindBPMNSid(tra.componentId, 1),
                                    outgoing = FindBPMNSid(tra.componentId, 3)
                                };

                                pNew.intermediateCatchMessageEvents.Add(icmNew);

                                // sequence Flow after

                                SimpleBPMN.SequenceFlow sfNew2 = new SimpleBPMN.SequenceFlow();

                                sfNew2.id = FindBPMNSid(tra.componentId, 3);
                                sfNew2.name = "";
                                sfNew2.sourceRef = FindBPMNSid(tra.componentId, 2);
                                sfNew2.targetRef = FindBPMNSid(tra.targetState.componentId, 1);

                                pNew.sequenceFlows.Add(sfNew2);
                            }
                            // Event Based Gateway AND DayTimeTimerTransition
                            else if (act.transitions.Count > 1 && act.state is PASS.ReceiveState && tra is PASS.DayTimeTimerTransition)
                            {
                                // sequence Flow before

                                SimpleBPMN.SequenceFlow sfNew1 = new SimpleBPMN.SequenceFlow();

                                sfNew1.id = FindBPMNSid(tra.componentId, 1);
                                sfNew1.name = "";
                                sfNew1.sourceRef = FindBPMNSid(tra.sourceState.componentId, 1);
                                sfNew1.targetRef = FindBPMNSid(tra.componentId, 2);

                                pNew.sequenceFlows.Add(sfNew1);

                                // incoming IntermediateCatchTimeEvent

                                SimpleBPMN.IntermediateCatchTimeEvent ictNew = new SimpleBPMN.IntermediateCatchTimeEvent
                                {
                                    id = FindBPMNSid(tra.componentId, 2),
                                    name = tra.transitionCondition.componentLabel,
                                    incoming = FindBPMNSid(tra.componentId, 1),
                                    outgoing = FindBPMNSid(tra.componentId, 3),
                                    timeDuration = tra.transitionCondition.duration
                                };

                                pNew.intermediateCatchTimeEvents.Add(ictNew);

                                // sequence Flow after

                                SimpleBPMN.SequenceFlow sfNew2 = new SimpleBPMN.SequenceFlow();

                                sfNew2.id = FindBPMNSid(tra.componentId, 3);
                                sfNew2.name = "";
                                sfNew2.sourceRef = FindBPMNSid(tra.componentId, 2);
                                sfNew2.targetRef = FindBPMNSid(tra.targetState.componentId, 1);

                                pNew.sequenceFlows.Add(sfNew2);
                            }
                            else
                            {
                                SimpleBPMN.SequenceFlow sfNew = new SimpleBPMN.SequenceFlow();

                                sfNew.id = FindBPMNSid(tra.componentId, 1);
                                sfNew.name = tra.componentLabel;
                                sfNew.sourceRef = FindBPMNSid(tra.sourceState.componentId, 1);
                                sfNew.targetRef = FindBPMNSid(tra.targetState.componentId, 1);

                                pNew.sequenceFlows.Add(sfNew);
                            }
                        }
                    }
                }
                newProcess.processes.Add(pNew); 
            }

            return newProcess;
        }

        public string FindBPMNSid(string text, int position)
        {
            // Find the position of the start of the SID
            int startIndex = text.IndexOf("sid-");

            if (position == 0)
            {
                return text.Substring(startIndex, 40);
            }
            else
            {
                //Remove everything to the end of the first SID
                text = text.Substring(startIndex + 40);

                // call the function again (recursion)
                return FindBPMNSid(text, position-1);
            }
        }

        public string FindSourceRef(List<PASS.SubjectBehavior> subjBes, PASS.MessageExchange me)
        {
            foreach (PASS.SubjectBehavior subjBe in subjBes)
            {
                foreach (PASS.Action act in subjBe.actions)
                {
                    if (act.transitions != null)
                    {
                        foreach (PASS.Transition tra in act.transitions)
                        {
                            if (tra.transitionCondition != null)
                            {
                                if (tra.transitionCondition.performedMessageExchange == me)
                                {
                                    if (tra is PASS.SendTransition)
                                    {
                                        return FindBPMNSid(tra.sourceState.componentId, 1);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return "error";  
        }

        public string FindTargetRef(List<PASS.SubjectBehavior> subjBes, PASS.MessageExchange me)
        {
            foreach (PASS.SubjectBehavior subjBe in subjBes)
            {
                foreach (PASS.Action act in subjBe.actions)
                {
                    if (act.transitions != null)
                    {
                        foreach (PASS.Transition tra in act.transitions)
                        {
                            if (tra.transitionCondition != null)
                            {
                                if (tra.transitionCondition.performedMessageExchange == me)
                                {
                                    // Event Based Gateway
                                    if (act.transitions.Count > 1 && tra is PASS.ReceiveTransition)
                                    {
                                        return FindBPMNSid(tra.componentId, 2);
                                    }
                                    // Normale States
                                    else if (tra is PASS.ReceiveTransition)
                                    {
                                        return FindBPMNSid(tra.sourceState.componentId, 1);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return "error";
        }

        public string FindSubjectName(string subjComId, List<PASS.FullySpecifiedSubject> subjs)
        {
            foreach (PASS.FullySpecifiedSubject subj in subjs)
            {
                if (subj.componentId == subjComId)
                {
                    return subj.componentLabel;
                }
            }
            return "Error";
        }

        public List<string> FindIncomings(string componentId, List<PASS.Action> acts)
        {
            List<string> incomings = new List<string>();

            foreach (PASS.Action act in acts)
            {
                if (act.transitions != null)
                {
                    foreach (PASS.Transition tra in act.transitions)
                    {
                        if (tra.targetState.componentId == componentId)
                        {
                            // Event Based Gateway
                            if (act.transitions.Count > 1 && act.state is PASS.ReceiveState)
                            {
                                incomings.Add(FindBPMNSid(tra.componentId, 3));
                            }
                            // Normale States
                            else
                            {
                                incomings.Add(FindBPMNSid(tra.componentId, 1));
                            } 
                        }
                    }
                }
            }
            return incomings;
        }

        bool IsStartSubject (List<PASS.SubjectBehavior> subjBes, string subjBeId)
        {
            foreach (PASS.SubjectBehavior subjBe in subjBes)
            {
                if (subjBe.componentId == subjBeId)
                {
                    foreach(PASS.Action act in subjBe.actions)
                    {
                        if (act.state.isStartState && act.state is PASS.DoState)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
