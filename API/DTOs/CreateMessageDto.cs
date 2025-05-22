using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class CreateMessageDto
{

    public required string RecipientUsername { get; set; }

    public required string Content { get; set; } 
}
