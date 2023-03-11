using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAppXml
{
    class PASS
    {
        public class PASSElements
        {
            public PASSProcessModel passProcessModel { get; set; }
            public ModelLayer modelLayer { get; set; }
            public List<FullySpecifiedSubject> subjects { get; set; }
            public List<MessageExchangeList> messageExchangeLists { get; set; }
            public List<SubjectBehavior> subjectBehaviors { get; set; }
        }


        public class SBPMElement
        {
            public string componentId { get; set; }
            public string componentLabel { get; set; }
            //public string bpmnId { get; set; }
        }

        public class PASSProcessModel : SBPMElement
        {
            public List<string> contains { get; set; }
        }

        public class ModelLayer : SBPMElement
        {
            public List<string> contains { get; set; }
        }

        public class FullySpecifiedSubject : SBPMElement
        {
            public string subjectBehavior { get; set; }
            public string bpmnId { get; set; }
        }

        public class MessageExchangeList : SBPMElement
        {
            public List<MessageExchange> messageExchanges { get; set; }
        }

        public class MessageExchange : SBPMElement
        {
            public string sender { get; set; }
            public string receiver { get; set; }
            public MessageSpecification messageSpecification { get; set; }
            public string bpmnId { get; set; }
        }

        public class MessageSpecification : SBPMElement
        {

        }

        public class SubjectBehavior : SBPMElement
        {
            public List<Action> actions { get; set; }
            public string subjectComponentId { get; set; }
        }

        public class Action : SBPMElement
        {
            public State state { get; set; }
            public List<Transition> transitions { get; set; }
            public string bpmnId { get; set; }
        }

        public class State : SBPMElement
        {
            public bool isStartState { get; set; }
            public bool isEndState { get; set; }
            public bool timeState = false;
        }

        public class DoState : State
        {
            
        }

        public class SendState : State
        {

        }

        public class ReceiveState : State
        {

        }

        public class Transition : SBPMElement
        {
            public State sourceState { get; set; }
            public State targetState { get; set; }
            public TransitionCondition transitionCondition { get; set; }
        }
        
        public class DoTransition : Transition
        {

        }

        public class SendTransition : Transition
        {

        }

        public class ReceiveTransition : Transition
        {

        }

        public class DayTimeTimerTransition : Transition
        {

        }
        

        public class TransitionCondition : SBPMElement
        {
            public MessageExchange performedMessageExchange { get; set; }
            public string messagSentTo { get; set; }
            public string messagSentFrom { get; set; }
            public string duration { get; set; }
        }
        
        public class SendTransitionCondition : TransitionCondition
        {
            
        }

        public class ReceiveTransitionCondition : TransitionCondition
        {
            

        }

        public class DayTimeTimerTransitionCondition : TransitionCondition
        {
            
        }
        
    }
}
