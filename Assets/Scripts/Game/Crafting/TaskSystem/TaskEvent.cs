public static class TaskEvent
{
    public struct ItemCrafted
    {
        public string itemId;
    }
    
    public struct ItemDelivered
    {
        public string itemId;
        public int quantity;
    }
}