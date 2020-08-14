using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PointOfSale.WebAPI.Models;
using PointOfSale.WebAPI.Repositories;
using PointOfSale.WebAPI.ViewModels.Requests;
using PointOfSale.WebAPI.ViewModels.Responses;

namespace PointOfSale.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly ILogger<ClientController> _logger;
        private readonly IClientRepository _clienteRepository;

        public ClientController(ILogger<ClientController> logger, IClientRepository clientRepository)
        {
            _logger = logger;
            _clienteRepository = clientRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            Response response = new Response();
            try
            {
                response.Data = await _clienteRepository.GetAllAsync();
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            Response response = new Response();
            try
            {
                response.Data = await _clienteRepository.GetByIdAsync(id);
                response.Success = true;
                if (response.Data == null)
                {
                    response.Success = false;
                    return NotFound(response);
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] ClientRequest request)
        {
            Response response = new Response();
            if (!ModelState.IsValid || request == null)
                return BadRequest();
            try
            {
                Client client = new Client()
                {
                    Name = request.Name
                };
                _clienteRepository.Add(client);
                await _clienteRepository.SaveAsync();
                response.Success = true;
                response.Data = client;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] ClientRequest request)
        {
            Response response = new Response();
            if (!ModelState.IsValid || request == null)
            {
                response.Success = false;
                response.Message = "Bad request";
                return BadRequest();
            }
            try
            {
                Client client = await _clienteRepository.GetByIdAsync(request.Id);
                if (client != null)
                {
                    client.Name = request.Name;
                    _clienteRepository.Update(client);
                    await _clienteRepository.SaveAsync();
                    response.Success = true;
                    response.Data = client;
                }
                else
                {
                    response.Success = false;
                    response.Message = $"The client {request.Id} doesn't exist";
                    return NotFound(response);
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            Response response = new Response();
            try
            {
                Client client = await _clienteRepository.GetByIdAsync(id);
                if (client != null)
                {
                    _clienteRepository.Remove(client);
                    await _clienteRepository.SaveAsync();
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                    response.Message = $"The client {id} doesn't exist";
                    return NotFound(response);
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }
    }
}
