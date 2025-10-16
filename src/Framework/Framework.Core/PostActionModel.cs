namespace Framework.Core
{
    public class PostActionModel
    {
        public Guid Id { get; set; }
    }

    public class PostActionModel<Key>
    {
        public Key Id { get; set; }
    }

    public class StatusPostActionModel : PostActionModel
    {
        public bool IsActive { get; set; }
    }
    public class StatusPostActionModel<Key> : PostActionModel<Key>
    {
        public bool IsActive { get; set; }
    }

    public class DeletePostActionModel : PostActionModel
    {
    }

    public class DeletePostActionModel<Key> : PostActionModel<Key>
    {
    }

    public class KeyValuePostActionModel<Key, TValue> : PostActionModel<Key>
    {
        public TValue Value { get; set; }
    }

    public class ValuePostActionModel
    {
        public string Value { get; set; }
    }
    public class ValuePostActionModel<TValue>
    {
        public TValue Value { get; set; }
    }
}
