using IT_05.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using static IT_05.Server.Models.view;

namespace IT_05.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QueueController : ControllerBase
    {

        [HttpGet(Name = "GetQueue")]
        public Queue Get()
        {
            using (var context = new IT_05Context())
            {
                var currentQueue = new Queue
                {
                    QueueNumber = "0",
                    CreateTime = DateTime.Now
                };
                try
                {
                    var LastPendingQueue = context.tickets.Where(x => x.status == "PENDING").OrderBy(x => x.id).Include(x => x.queue).Last();
                    currentQueue = new Queue
                    {
                        QueueNumber = LastPendingQueue.queue.queue_number,
                        CreateTime = LastPendingQueue.created_at.Value.DateTime
                    };
                    return currentQueue;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                    return currentQueue;
                }
            }
        }

        [HttpPost(Name = "CreateQueue")]
        public async Task<Queue> Create()
        {
            try
            {
                var NewQueue = new ticket
                { 
                    queue_id = 1,
                    created_at = DateTime.Now,
                    status = "PENDING"
                };

                using (var context = new IT_05Context())
                {
                    var QueueExist = context.tickets.Where(x => x.created_at >= DateTime.Today ).ToList();

                    if(QueueExist.Count == 0)
                    {
                        NewQueue.queue_id = 1;
                    }
                    else
                    {
                        var lastQueueId = context.tickets.OrderBy(x => x.queue_id).Last().queue_id;
                        if ( lastQueueId != 260)
                        { 
                            var newQueueId = lastQueueId + 1;
                            NewQueue.queue_id = (int)newQueueId;
                        }
                    }

                    using var transaction = await context.Database.BeginTransactionAsync();
                    try
                    {
                        context.tickets.Add(NewQueue);
                        await context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        var currentQueue = context.tickets.Where(x => x.id == NewQueue.id).Select(x => new Queue
                        {
                            QueueNumber = x.queue.queue_number,
                            CreateTime = x.created_at.Value.DateTime
                        }).FirstOrDefault();
                        return currentQueue;
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync(); // Revert changes on error
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return new Queue
                {
                    QueueNumber = "Error",
                    CreateTime = DateTime.Now
                }; ;
            }
        }

        [HttpDelete(Name = "DeleteQueue")]
        public async Task<Queue> Delete()
        {
            try
            {
                using (var context = new IT_05Context())
                {
                    var QueueExist = context.tickets.Where(x => x.created_at >= DateTime.Today).ToList();
                    using var transaction = await context.Database.BeginTransactionAsync();
                    try
                    {
                        context.tickets.RemoveRange(QueueExist);
                        await context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        return new Queue
                        {
                            QueueNumber = "00",
                            CreateTime = DateTime.Now
                        };
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync(); // Revert changes on error
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return new Queue
                {
                    QueueNumber = "Error",
                    CreateTime = DateTime.Now
                }; ;
            }
        }
    }
}
