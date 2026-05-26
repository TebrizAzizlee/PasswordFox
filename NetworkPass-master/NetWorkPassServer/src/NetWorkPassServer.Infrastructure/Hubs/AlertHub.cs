using Microsoft.AspNetCore.SignalR;


namespace NetWorkPassServer.Infrastructure.Hubs
{
    public sealed class AlertHub:Hub
    {
        public override async Task OnConnectedAsync()
        {
            // 🔥 default noc dashboard group

            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                "noc");

            await base.OnConnectedAsync();
        }

        public async Task JoinBranchGroup(
            Guid branchId)
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                $"branch:{branchId}");
        }

        public async Task LeaveBranchGroup(
            Guid branchId)
        {
            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                $"branch:{branchId}");
        }
    }
}
