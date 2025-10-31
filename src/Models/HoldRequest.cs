public class HoldRequest {
    public int RequestId { get; set; }
    public int BookId { get; set; }
    public int MemberId { get; set; }
    public DateTime RequestedAt { get; set; }
    public string Status { get; set; } = "pending"; // pending, approved, denied
}
