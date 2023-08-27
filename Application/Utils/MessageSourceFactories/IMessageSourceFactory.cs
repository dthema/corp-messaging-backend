using Application.DTO.Messages;
using Application.DTO.MessageSources;
using Application.DTO.Workers;
using Application.Utils.WorkerFactories;
using Domain.Models.Messages;
using Domain.Models.MessageSources;
using Domain.Models.Workers;

namespace Application.Utils.MessageSourceFactories;

public interface IMessageSourceFactory
{
    MessageSourceDto CreateMessageSourceDto(MessageSource messageSource);
    IMessageSourceFactory AddNext(IMessageSourceFactory messageSourceFactory);
}