using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PointOfSale.WebAPI.Controllers;
using PointOfSale.WebAPI.Models;
using PointOfSale.WebAPI.Repositories;
using PointOfSale.WebAPI.ViewModels.Requests;
using PointOfSale.WebAPI.ViewModels.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PointOfSale.WebAPI.Tests
{
    public class ClientControllerTest
    {
        private Mock<ILogger<ClientController>> _mockLogger;

        private Mock<IClientRepository> _mockRepository;

        public ClientControllerTest()
        {
            _mockLogger = new Mock<ILogger<ClientController>>();
            _mockRepository = new Mock<IClientRepository>();
    }

        [Fact]
        public async Task GetAllClientsReturnAllClientsAsync()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(GetClients);
            
            // Act
            var controller = new ClientController(_mockLogger.Object, _mockRepository.Object);
            var result = await controller.GetAsync();

            // Assert
            _mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response>(okObjectResult?.Value);
            var clients = Assert.IsType<List<Client>>(response?.Data);
            Assert.Equal(_mockRepository.Object.GetAllAsync().Result.Count(), clients?.Count);
            Assert.True(response.Success);
        }

        [Fact]
        public async Task GetAllClientsThrowExceptionAsync()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAllAsync()).Throws(new Exception("Error"));

            // Act
            var controller = new ClientController(_mockLogger.Object, _mockRepository.Object);
            var result = await controller.GetAsync();

            // Assert
            _mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response>(okObjectResult?.Value);
            Assert.Null(response?.Data);
            Assert.Equal("Error", response?.Message);
            Assert.False(response.Success);
        }

        [Theory]
        [InlineData(1)]
        public async Task GetClientByIdReturnOkAsync(int id)
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(GetClients().First(c => c.Id == id));

            // Act
            var controller = new ClientController(_mockLogger.Object, _mockRepository.Object);
            var result = await controller.GetAsync(id);

            // Assert
            _mockRepository.Verify(repo => repo.GetByIdAsync(id), Times.Once);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response>(okObjectResult?.Value);
            var client = Assert.IsType<Client>(response?.Data);
            Assert.Equal(id, client?.Id);
            Assert.True(response.Success);
        }

        [Theory]
        [InlineData(5)]
        public async Task GetClientByIdReturnNotFoundAsync(int id)
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(GetClients().FirstOrDefault(c => c.Id == id));

            // Act
            var controller = new ClientController(_mockLogger.Object, _mockRepository.Object);
            var result = await controller.GetAsync(id);

            // Assert
            _mockRepository.Verify(repo => repo.GetByIdAsync(id), Times.Once);
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<Response>(notFound?.Value);
            Assert.Null(response?.Data);
            Assert.False(response.Success);
        }

        [Fact]
        public async Task AddClientReturnOkObjectResultCreatingValidClient()
        {
            // Arrange
            ClientRequest request = new ClientRequest()
            {
                Name = "New Client"
            };
            _mockRepository.Setup(repo => repo.Add(It.IsAny<Client>())).Verifiable();
            _mockRepository.Setup(repo => repo.SaveAsync()).Verifiable();

            // Act
            var controller = new ClientController(_mockLogger.Object, _mockRepository.Object);
            var result = await controller.AddAsync(request);

            // Assert
            _mockRepository.Verify(repo => repo.Add(It.IsAny<Client>()), Times.Once);
            _mockRepository.Verify(repo => repo.SaveAsync(), Times.Once);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response>(okObjectResult?.Value);
            var client = Assert.IsType<Client>(response?.Data);
            Assert.Equal(request.Name, client.Name);
            Assert.True(response.Success);
        }
        
        [Fact]
        public async Task AddClientReturnBadRequestCreatingNullClient()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.Add(null)).Verifiable();
            _mockRepository.Setup(repo => repo.SaveAsync()).Verifiable();

            // Act
            var controller = new ClientController(_mockLogger.Object, _mockRepository.Object);
            var result = await controller.AddAsync(null);

            // Assert
            _mockRepository.Verify(repo => repo.Add(It.IsAny<Client>()), Times.Never);
            _mockRepository.Verify(repo => repo.SaveAsync(), Times.Never);
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task AddClientReturnOkObjectResultWhenRepositoryThrowException()
        {
            // Arrange
            ClientRequest request = new ClientRequest()
            {
                Name = "New Client"
            };
            _mockRepository.Setup(repo => repo.Add(It.IsAny<Client>())).Throws(new Exception("Error"));
            _mockRepository.Setup(repo => repo.SaveAsync()).Verifiable();

            // Act
            var controller = new ClientController(_mockLogger.Object, _mockRepository.Object);
            var result = await controller.AddAsync(request);

            // Assert
            _mockRepository.Verify(repo => repo.Add(It.IsAny<Client>()), Times.Once);
            _mockRepository.Verify(repo => repo.SaveAsync(), Times.Never);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response>(okObjectResult?.Value);
            Assert.Null(response?.Data);
            Assert.False(response?.Success);
        }

        [Fact]
        public async Task UpdateClientReturnOkObjectResultUpdatingValidClient()
        {
            // Arrange
            ClientRequest request = new ClientRequest()
            {
                Id = 1,
                Name = "Update Client"
            };
            _mockRepository.Setup(repo => repo.GetByIdAsync(request.Id))
                .ReturnsAsync(GetClients().FirstOrDefault(c => c.Id == request.Id)).Verifiable();
            _mockRepository.Setup(repo => repo.Update(It.IsAny<Client>())).Verifiable();
            _mockRepository.Setup(repo => repo.SaveAsync()).Verifiable();

            // Act
            var controller = new ClientController(_mockLogger.Object, _mockRepository.Object);
            var result = await controller.UpdateAsync(request);

            // Assert
            _mockRepository.Verify(repo => repo.GetByIdAsync(request.Id), Times.Once);
            _mockRepository.Verify(repo => repo.Update(It.IsAny<Client>()), Times.Once);
            _mockRepository.Verify(repo => repo.SaveAsync(), Times.Once);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response>(okObjectResult?.Value);
            var client = Assert.IsType<Client>(response?.Data);
            Assert.Equal(request.Name, client.Name);
            Assert.Equal(request.Id, client.Id);
            Assert.True(response.Success);
        }

        [Fact]
        public async Task UpdateClientReturnNotFoundResultUpdatingValidClient()
        {
            // Arrange
            ClientRequest request = new ClientRequest()
            {
                Id = 5,
                Name = "Update Client"
            };
            _mockRepository.Setup(repo => repo.GetByIdAsync(request.Id))
                .ReturnsAsync(GetClients().FirstOrDefault(c => c.Id == request.Id)).Verifiable();
            _mockRepository.Setup(repo => repo.Update(It.IsAny<Client>())).Verifiable();
            _mockRepository.Setup(repo => repo.SaveAsync()).Verifiable();

            // Act
            var controller = new ClientController(_mockLogger.Object, _mockRepository.Object);
            var result = await controller.UpdateAsync(request);

            // Assert
            _mockRepository.Verify(repo => repo.GetByIdAsync(request.Id), Times.Once);
            _mockRepository.Verify(repo => repo.Update(It.IsAny<Client>()), Times.Never);
            _mockRepository.Verify(repo => repo.SaveAsync(), Times.Never);
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<Response>(notFound?.Value);
            Assert.Null(response?.Data);
            Assert.False(response.Success);
        }

        [Fact]
        public async Task UpdateClientReturnBadRequestUpdatingNullClient()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.Update(null)).Verifiable();
            _mockRepository.Setup(repo => repo.SaveAsync()).Verifiable();

            // Act
            var controller = new ClientController(_mockLogger.Object, _mockRepository.Object);
            var result = await controller.UpdateAsync(null);

            // Assert
            _mockRepository.Verify(repo => repo.Update(It.IsAny<Client>()), Times.Never);
            _mockRepository.Verify(repo => repo.SaveAsync(), Times.Never);
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task UpdateClientReturnOkObjectResultWhenRepositoryThrowException()
        {
            // Arrange
            ClientRequest request = new ClientRequest()
            {
                Id = 1,
                Name = "Update Client"
            };
            _mockRepository.Setup(repo => repo.GetByIdAsync(request.Id))
                .ReturnsAsync(GetClients().FirstOrDefault(c => c.Id == request.Id)).Verifiable();
            _mockRepository.Setup(repo => repo.Update(It.IsAny<Client>())).Throws(new Exception("Error"));
            _mockRepository.Setup(repo => repo.SaveAsync()).Verifiable();

            // Act
            var controller = new ClientController(_mockLogger.Object, _mockRepository.Object);
            var result = await controller.UpdateAsync(request);

            // Assert
            _mockRepository.Verify(repo => repo.GetByIdAsync(request.Id), Times.Once);
            _mockRepository.Verify(repo => repo.Update(It.IsAny<Client>()), Times.Once);
            _mockRepository.Verify(repo => repo.SaveAsync(), Times.Never);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response>(okObjectResult?.Value);
            Assert.Null(response?.Data);
            Assert.False(response?.Success);
        }

        [Theory]
        [InlineData(1)]
        public async Task DeleteClientReturnOkObjectResultUpdatingValidClient(int id)
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(GetClients().FirstOrDefault(c => c.Id == id)).Verifiable();
            _mockRepository.Setup(repo => repo.Remove(It.IsAny<Client>())).Verifiable();
            _mockRepository.Setup(repo => repo.SaveAsync()).Verifiable();

            // Act
            var controller = new ClientController(_mockLogger.Object, _mockRepository.Object);
            var result = await controller.DeleteAsync(id);

            // Assert
            _mockRepository.Verify(repo => repo.GetByIdAsync(id), Times.Once);
            _mockRepository.Verify(repo => repo.Remove(It.IsAny<Client>()), Times.Once);
            _mockRepository.Verify(repo => repo.SaveAsync(), Times.Once);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response>(okObjectResult?.Value);
            Assert.Null(response?.Data);
            Assert.True(response?.Success);
        }

        [Theory]
        [InlineData(5)]
        public async Task DeleteClientReturnNotFoundValidClient(int id)
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(GetClients().FirstOrDefault(c => c.Id == id)).Verifiable();
            _mockRepository.Setup(repo => repo.Remove(It.IsAny<Client>())).Verifiable();
            _mockRepository.Setup(repo => repo.SaveAsync()).Verifiable();

            // Act
            var controller = new ClientController(_mockLogger.Object, _mockRepository.Object);
            var result = await controller.DeleteAsync(id);

            // Assert
            _mockRepository.Verify(repo => repo.GetByIdAsync(id), Times.Once);
            _mockRepository.Verify(repo => repo.Remove(It.IsAny<Client>()), Times.Never);
            _mockRepository.Verify(repo => repo.SaveAsync(), Times.Never);
            var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<Response>(notFoundObjectResult?.Value);
            Assert.Null(response?.Data);
            Assert.False(response?.Success);
        }

        [Theory]
        [InlineData(5)]
        public async Task DeleteClientOkObjectResultWhenRepositoryThrowException(int id)
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetByIdAsync(id)).Throws(new Exception("Error"));
            _mockRepository.Setup(repo => repo.Remove(It.IsAny<Client>())).Verifiable();
            _mockRepository.Setup(repo => repo.SaveAsync()).Verifiable();

            // Act
            var controller = new ClientController(_mockLogger.Object, _mockRepository.Object);
            var result = await controller.DeleteAsync(id);

            // Assert
            _mockRepository.Verify(repo => repo.GetByIdAsync(id), Times.Once);
            _mockRepository.Verify(repo => repo.Remove(It.IsAny<Client>()), Times.Never);
            _mockRepository.Verify(repo => repo.SaveAsync(), Times.Never);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response>(okObjectResult?.Value);
            Assert.Null(response?.Data);
            Assert.False(response?.Success);
            Assert.Equal("Error", response?.Message);
        }

        private List<Client> GetClients()
        {
            var list = new List<Client>();
            list.Add(new Client()
            {
                Id = 1,
                Name = "Yako"
            });
            list.Add(new Client()
            {
                Id = 2,
                Name = "Nahla"
            });
            list.Add(new Client()
            {
                Id = 3,
                Name = "Volteretas"
            });
            list.Add(new Client()
            {
                Id = 4,
                Name = "Pajarito"
            });
            return list;
        }
    }
}
