namespace BotZeitNot.Domain.Interface
{
    public interface IEntityDto<TKey>
    {
        TKey Id { get; set; }
    }
}
