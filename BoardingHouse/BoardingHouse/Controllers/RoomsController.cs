using BoardingHouse.Application.Common.Models;
using BoardingHouse.Application.Rooms;
using Microsoft.AspNetCore.Mvc;

namespace BoardingHouse.API.Controllers;

[ApiController]
[Route("api/rooms")]
public sealed class RoomsController(IRoomService roomService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<PagedResult<RoomDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<RoomDto>>> GetPage(
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default) =>
        Ok(await roomService.GetPageAsync(pageNumber, pageSize, cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType<RoomDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RoomDto>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await roomService.GetByIdAsync(id, cancellationToken));
}
