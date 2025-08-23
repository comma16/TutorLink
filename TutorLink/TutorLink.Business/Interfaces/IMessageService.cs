using TutorLink.Data.Entities;

namespace TutorLink.Business.Interfaces;

public interface IMessageService
{
    Task<Mensaje> SendAsync(Mensaje m);
    Task<List<Mensaje>> InboxAsync(int studentId);
}