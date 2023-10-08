// A Token would likely want to be used elsehwere

namespace EventStreamProcessor
{
    internal class Token
    {
        public string? Id { get; set; }
        public string? Owner { get; set; }
    }
}
