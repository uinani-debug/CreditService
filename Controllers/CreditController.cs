using AutoMapper;
using CreditLibrary.API.Models;
using CreditLibrary.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Confluent.Kafka;
using PaymentService.API.Models;
using Newtonsoft.Json;
using System.Threading;
using CreditLibrary.API.Entities;

namespace CreditLibrary.API.Controllers
{
    [ApiController]

    public class CreditController : ControllerBase
    {
        private readonly ICreditLibraryRepository _PaymentLibraryRepository;
        private readonly IMapper _mapper;
        private readonly ConsumerConfig _config;
        public CreditController(ICreditLibraryRepository PaymentLibraryRepository,
            IMapper mapper)
        {



            _PaymentLibraryRepository = PaymentLibraryRepository ??
                throw new ArgumentNullException(nameof(PaymentLibraryRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [Route("Credit")]
        [HttpPost]
        public ActionResult<string> Credit()
        {
            SubscribeEvent();
            return Accepted();
        }


        [Route("TestOracle")]
        [HttpGet]
        public ActionResult<string> Test()
        {
            var CreditRequest = new Credit
            {
                AccountIdentifier = "54678947",
                PaymentReference = "test credit oracle1",
                TransferAmount = 100
            };

            if (Request != null)
            {
                _PaymentLibraryRepository.CreditAmount(CreditRequest);
            }

                return Accepted();
        }


        private async void SubscribeEvent()
        {
            var _config = new ConsumerConfig
            {
                GroupId = "test-consumer-group",
                BootstrapServers = "3.129.43.25:9092",
                // Note: The AutoOffsetReset property determines the start offset in the event
                // there are not yet any committed offsets for the consumer group for the
                // topic/partitions of interest. By default, offsets are committed
                // automatically, so in this example, consumption will only start from the
                // earliest message in the topic 'my-topic' the first time you run the program.
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var c = new ConsumerBuilder<Ignore, string>(_config).Build())
            {
                c.Subscribe("CreditAccount");

                CancellationTokenSource cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) => {
                    e.Cancel = true; // prevent the process from terminating.
                    cts.Cancel();
                };

                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = c.Consume(cts.Token);
                            var Request = JsonConvert.DeserializeObject<CreditRequest>(cr.Value);
                            var CreditRequest = new Credit
                            {
                                AccountIdentifier = Request.Creditor.AccountIdentifier,
                                PaymentReference = Request.PaymentReference,
                                TransferAmount = Request.TransferAmount.Amount
                            };

                            if(Request != null)
                            {
                                _PaymentLibraryRepository.CreditAmount(CreditRequest);
                                using (var p = new ProducerBuilder<Null, string>(_config).Build())
                                {
                                    try
                                    {
                                        string jsonRequest = JsonConvert.SerializeObject(CreditRequest);
                                        var dr1 = await p.ProduceAsync("AccountCredited", new Message<Null, string> { Value = jsonRequest });                                        
                                    }
                                    catch (ProduceException<Null, string> e)
                                    {                                        
                                    }
                                }
                                                           }
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    c.Close();
                }
            }
        }


        public override ActionResult ValidationProblem(
            [ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices
                .GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
    }
}
