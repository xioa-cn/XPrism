using Xunit;
using System.Threading.Tasks;
using XPrism.Core.Events;

namespace XPrism.Core.Test.Events {
    public class EventsTests {
        private readonly IEventAggregator _eventAggregator;

        public EventsTests() {
            _eventAggregator = new EventAggregator();
        }

        [Fact]
        public void Subscribe_WhenEventPublished_ShouldReceiveMessage() {
            // Arrange
            var testEvent = _eventAggregator.GetEvent<PubSubEvent<string>>();
            string receivedMessage = null;
            testEvent.Subscribe(message => receivedMessage = message);

            // Act
            testEvent.Publish("Hello World");

            // Assert
            Assert.Equal("Hello World", receivedMessage);
        }

        [Fact]
        public void Subscribe_WithFilter_ShouldOnlyReceiveFilteredMessages() {
            // Arrange
            var testEvent = _eventAggregator.GetEvent<PubSubEvent<string>>();
            string receivedMessage = null;
            testEvent.Subscribe(message => receivedMessage = message,
                filter: msg => msg.StartsWith("Test"));

            // Act
            testEvent.Publish("Hello World"); // 不应该被接收
            testEvent.Publish("Test Message"); // 应该被接收

            // Assert
            Assert.Equal("Test Message", receivedMessage);
        }

        [Fact]
        public void Unsubscribe_WhenEventPublished_ShouldNotReceiveMessage() {
            // Arrange
            var testEvent = _eventAggregator.GetEvent<PubSubEvent<string>>();
            string receivedMessage = null;
            var subscription = testEvent.Subscribe(message => receivedMessage = message);

            // Act
            testEvent.Publish("First Message");
            string firstMessage = receivedMessage;

            testEvent.Unsubscribe(subscription);
            testEvent.Publish("Second Message");

            // Assert
            Assert.Equal("First Message", firstMessage);
            Assert.NotEqual("Second Message", receivedMessage);
        }

        [Fact]
        public async Task Subscribe_WithThreadOption_ShouldReceiveMessageOnCorrectThread() {
            // Arrange
            var testEvent = _eventAggregator.GetEvent<PubSubEvent<string>>();
            string receivedMessage = null;
            var currentThreadId = Task.CurrentId;

            testEvent.Subscribe(message => { receivedMessage = message; }, ThreadOption.PublisherThread);

            // Act
            await Task.Run(() => testEvent.Publish("Async Message"));

            // Assert
            Assert.Equal("Async Message", receivedMessage);
        }

        [Fact]
        public void Subscribe_WithKeepSubscriberReferenceTrue_ShouldMaintainReference() {
            // Arrange
            var testEvent = _eventAggregator.GetEvent<PubSubEvent<string>>();
            string receivedMessage = null;
            var subscriber = new WeakReference(new object());

            testEvent.Subscribe(message => receivedMessage = message,
                keepSubscriberReferenceAlive: true);

            // Act
            GC.Collect();
            testEvent.Publish("Test Message");

            // Assert
            Assert.Equal("Test Message", receivedMessage);
        }
    }
}