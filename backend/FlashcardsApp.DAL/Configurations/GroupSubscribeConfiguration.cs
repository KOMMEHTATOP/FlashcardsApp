using FlashcardsApp.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashcardsApp.DAL.Configurations;

public class GroupSubscribeConfiguration :IEntityTypeConfiguration<UserGroupSubscription>
{
    public void Configure(EntityTypeBuilder<UserGroupSubscription> builder) 
    {
        builder.HasKey(ugs => ugs.Id);
        
        builder.HasIndex(ugs => new { ugs.SubscriberUserId, ugs.GroupId })
            .IsUnique()
            .HasDatabaseName("IX_UserGroupSubscriptions_User_Group");
        
        builder.HasOne(ugs => ugs.SubscriberUser)
            .WithMany(u => u.Subscriptions)
            .HasForeignKey(ugs => ugs.SubscriberUserId)
            .OnDelete(DeleteBehavior.Cascade);
              
        builder.HasOne(ugs => ugs.Group)
            .WithMany(g => g.Subscriptions)
            .HasForeignKey(ugs => ugs.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }

}
