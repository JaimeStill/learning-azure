# Azure Queue Storage

Storage queues are part of the Azure Storage infrastructure. They allow you to store large numbers of messages. You can access messages from anywhere in the world via authenticated calls using HTTP or HTTPS. A queue message can be up to 64 KB in size. A queue may contain millions of messages, up to the total capacity limit of a storage account. Queues are commonly used to create a backlog of work to process asynchronously.

## Usage Considerations

As a solution architect / developer, **you should consider using Storage queues** when:

* Your application must store over 80 gigabytes of messages in a queue.

* Your application wants to track progress for processing a message in the queue. It's useful if the worker processing a message crashes. Another worker can then use that information to continue from where the prior worker left off.

* You require server side logs of all of the transactions executed against your queues.