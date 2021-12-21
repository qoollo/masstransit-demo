using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading.Tasks;
using ApiService.Contracts.ManagerApi;
using ApiService.Contracts.MonitoringApi;
using ApiService.Contracts.UserApi;
using ApiService.Models.Interfaces;
using CartService.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApiService.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IRoutingConfiguration _routingConfiguration;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IRequestClient<GetAllOrdersState> _allOrderStatesClient;
        private readonly IRequestClient<GetOrderState> _orderStateClient;
        private readonly IRequestClient<GetArchivedOrder> _archiverOrderClient;
        private readonly IRequestClient<AbortOrder> _abortOrderClient;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(ISendEndpointProvider sendEndpointProvider, 
            IRoutingConfiguration routingConfiguration, 
            IPublishEndpoint publishEndpoint, 
            IRequestClient<GetAllOrdersState> allOrderStatesClient,
            IRequestClient<GetOrderState> orderStateClient,
            IRequestClient<GetArchivedOrder> archiverOrderClient,
            IRequestClient<AbortOrder> abortOrderClient,
            ILogger<OrdersController> logger)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _routingConfiguration = routingConfiguration;
            _publishEndpoint = publishEndpoint;
            _allOrderStatesClient = allOrderStatesClient;
            _orderStateClient = orderStateClient;
            _archiverOrderClient = archiverOrderClient;
            _abortOrderClient = abortOrderClient;
            _logger = logger;
        }

        [HttpPost]
        [Route("{id}/cart-position/add")]
        public async Task<IActionResult> PostCartPosition([FromRoute][Required] Guid id, 
            [FromQuery][Required] string name,
            [FromQuery][Required] int amount)
        {
            try
            {
                var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri(_routingConfiguration.CartServiceAddress!));

                await endpoint.Send<AddCartPosition>(new
                {
                    OrderId = id,
                    Amount = amount,
                    Name = name,
                });

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500, e);
            }
        }

        [HttpPost]
        [Route("{id}/submit")]
        public async Task<IActionResult> PostCartPosition([FromRoute][Required] Guid id)
        {
            try
            {
                await _publishEndpoint.Publish<OrderSubmitted>(new
                {
                    OrderId = id
                });

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500, e);
            }
        }

        [HttpPost]
        [Route("{id}/confirm")]
        public async Task<IActionResult> SubmitOrder([FromRoute][Required] Guid id,
            [FromQuery][Required] string name)
        {
            try
            {
                await _publishEndpoint.Publish<ConfirmOrder>(new
                {
                    OrderId = id,
                    ConfirmManager = name
                });

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                return StatusCode(500, e);
            }
        }

        [HttpPost]
        [Route("{id}/reject")]
        public async Task<IActionResult> RejectOrder([FromRoute][Required] Guid id,
            [FromQuery][Required] string name,
            [FromQuery][Required] string reason)
        {
            try
            {
                await _publishEndpoint.Publish<RejectOrder>(new
                {
                    OrderId = id,
                    RejectManager = name,
                    Reason = reason
                });

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                return StatusCode(500, e);
            }
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetAllOrdersState()
        {
            try
            {
                var response = await _allOrderStatesClient.GetResponse<GetAllOrdersStateResponse>(new { });

                return Ok(response.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                return StatusCode(500, e);
            }
        }

        [HttpGet]
        [Route("{id}/state")]
        public async Task<IActionResult> GetOrderState([FromRoute][Required] Guid id)
        {
            try
            {
                var response = await _orderStateClient.GetResponse<GetOrderStateResponse>(new
                {
                    OrderId = id
                });

                return Ok(response.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                return StatusCode(500, e);
            }
        }

        [HttpPost]
        [Route("{id}/send-feedback")]
        public async Task<IActionResult> GetOrderState([FromRoute][Required] Guid id,
            [FromQuery][Required] string text,
            [FromQuery][Required] int startAmount)
        {
            try
            {
                await _publishEndpoint.Publish<FeedbackReceived>(new
                {

                    OrderId = id,
                    Text = text,
                    StarsAmount = startAmount
                });

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                return StatusCode(500, e);
            }
        }

        [HttpPost]
        [Route("{id}/archive")]
        public async Task<IActionResult> GetOrderArchiveState([FromRoute][Required] Guid id)
        {
            try
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                /*await _publishEndpoint.Publish<GetArchivedOrder>(new
                {
                    OrderId = id
                });*/

                var archivedOrder = (await _archiverOrderClient.GetResponse<GetArchivedOrderResponse>(new
                {
                    OrderId = id
                })).Message;

                stopWatch.Stop();

                return Ok(new { archivedOrder, stopWatch.ElapsedMilliseconds});
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                return StatusCode(500, e);
            }
        }

        [HttpPost]
        [Route("{id}/abort")]
        public async Task<IActionResult> AbortOrder([FromRoute][Required] Guid id)
        {
            try
            {
                var abortedOrder = (await _abortOrderClient.GetResponse<OrderAborted>(new
                {
                    OrderId = id
                })).Message;

                return Ok(abortedOrder);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                return StatusCode(500, e);
            }
        }
    }
}
