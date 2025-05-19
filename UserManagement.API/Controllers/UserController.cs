using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Core;
using UserManagement.Consumers;
using UserManagement.Domain.Entities;
using UserManagement.Domain.EventContracts;
using UserManagement.Domain.EventJobContracts;
using UserManagement.Domain.DTO;
using UserManagement.Infrastructure.Database;
using MassTransit;

namespace UserManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly GenericProducer _producer;
        private readonly GenericProducer _JobProducer;

        public UserController(IUserService service, GenericProducer producer, IPublishEndpoint publishEndpoint)
        {
            this._service = service;
            this._producer = producer;
            this._publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAll() => Ok(_service.GetAll());

        [HttpGet("{id}")]
        public ActionResult<User> GetById(Guid id)
        {
            var user = _service.GetById(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            try
            {
                // first process
                // for User.Created
                _service.Create(user);

                //// This publish for Publishing without context-aware
                //await _producer.PublishAsync(new UserCreatedEvent { User = user });

                // This publish for Publishing with context-aware for Outbox
                await _producer.PublishAsync(new UserCreatedEvent { User = user });
                await _producer.PublishAsync(new UserUpdatedEvent { User = user });

                //await _producer.SendAsync(new UserCreatedEvent { User = user }, "UserCreatedConsumer_Queue");

                return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                // Rollback the transaction if any error occurs
                //await transaction.RollbackAsync();
                throw;
            }



        }
        [HttpPost("CreateUsingJobConsumer")]
        public async Task<IActionResult> CreateUsingJobConsumer(User user)
        {
            try
            {
                // first process
                _service.Create(user);

                // second process
                await _JobProducer.PublishAsync(new ProcessUserJob
                {
                    UserId = user.Id,
                    JobType = "UserCreation",
                    Payload = user
                });
                return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, User user)
        {
            if (id != user.Id) return BadRequest();
            _service.Update(user);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            _service.Delete(id);
            return NoContent();
        }
    }
}