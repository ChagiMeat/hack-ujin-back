﻿using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Net.Http;
using System.Net.Http.Json;
using Newtonsoft.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

namespace XakUjin2025.Controllers
{
    // Определение маршрута и атрибута контроллера для данного класса контроллера.
    [Route("api/v1/[controller]")]
    [ApiController]
    public class GetController : Controller
    {
        // Объявляются частные поля для контекста базы данных и вспомогательного класса для работы с токенами.
        private readonly ApplicationDbContext _context;
        private readonly TokenHelper _tokenHelper;

        // Конструктор контроллера с внедрением зависимостей для контекста базы данных и вспомогательного класса токенов.
        public GetController(ApplicationDbContext context, TokenHelper tokenHelper)
        {
            _context = context;
            _tokenHelper = tokenHelper;
        }

        // Метод действия контроллера для получения имени пользователя из токена.
        [HttpPost("username-from-token")]
        public async Task<IActionResult> GetUserNameFromToken([FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            try
            {
                // Извлечение имени пользователя из токена.
                if (_tokenHelper.IsTokenExpired(authorizationHeader))
                    return Unauthorized(new { Message = "Expired token" });

                // Получение идентификатора текущего токена.
                var currentTokenId = _tokenHelper.GetCurrentTokenId(authorizationHeader);

                // Проверка действительности токена.
                if (_tokenHelper.IsInvalidToken(currentTokenId))
                    return Unauthorized(new { Message = "This token has been invalidated." });

                // Извлечение имени пользователя из токена.
                var username = _tokenHelper.ExtractUsernameFromToken(Request.Headers["Authorization"].ToString());


                // Если имя пользователя найдено, возвращается ответ с именем пользователя.
                if (username != null)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                    return Ok(new { user });
                }
                // В противном случае возвращается ответ об ошибке.
                else
                    return Unauthorized(new { Message = "Invalid token" });
            }
            // Обработка исключений и возврат ошибки сервера.
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { Error = "An error occurred while processing your request. Please try again later." });
            }
        }

        [HttpGet("indications")]
        public async Task<IActionResult> GetReadings([FromQuery] string signalSN, [FromQuery] string? startDate, string? endDate)
        {
            if (string.IsNullOrWhiteSpace(signalSN))
            {
                return BadRequest(new { Error = "Parameter signalSN is required and must be non-zero." });
            }

            try
            {
                var existingSignal= await _context.Signals
                    .FirstOrDefaultAsync(s => s.SignalSN == signalSN);

                if (existingSignal == null)
                {
                    return BadRequest(new { Error = "This signalSN dosn'texist" });
                }


                var indications = _context.Indications
                    .Where(r => r.SignalId == existingSignal.SignalId);

                if (!string.IsNullOrWhiteSpace(startDate) && !string.IsNullOrWhiteSpace(endDate))
                {
                    if (DateTime.TryParse(startDate, out DateTime parsedStartDate) && DateTime.TryParse(endDate, out DateTime parsedEndDate))
                    {
                        indications = indications.Where(r => r.CreatedAt.Date > parsedStartDate.Date && r.CreatedAt < parsedEndDate);
                    }
                    else
                    {
                        return BadRequest(new { Error = "Date parameter is not a valid date." });
                    }
                }
                else if (!string.IsNullOrWhiteSpace(startDate))
                {
                    if (DateTime.TryParse(startDate, out DateTime parsedStartDate))
                    {
                        indications = indications.Where(r => r.CreatedAt.Date > parsedStartDate.Date);
                    }
                    else
                    {
                        return BadRequest(new { Error = "Date parameter is not a valid date." });
                    }
                }
                else if (!string.IsNullOrWhiteSpace(endDate))
                {
                    if (DateTime.TryParse(endDate, out DateTime parsedEndDate))
                    {
                        indications = indications.Where(r => r.CreatedAt.Date < parsedEndDate.Date);
                    }
                    else
                    {
                        return BadRequest(new { Error = "Date parameter is not a valid date." });
                    }
                }

                var readings = await indications
                    .Select(i => new IndicationDto
                    {
                        IndicationId = i.IndicationId,
                        IndicationLabel = i.IndicationLabel,
                        IndicationName = i.IndicationName,
                        IndicationValue = i.IndicationValue,
                        CreatedAt = i.CreatedAt,
                    })
                    .ToListAsync();

                return Ok(readings);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return StatusCode(500, new { Error = "An error occurred while processing your request." });
            }
        }
    }
}
