using BootCamp.TaskService.Domain.Entity;

namespace BootCamp.TaskService.Web.GrapohQL.Types;

public sealed class TaskType : ObjectType<UserTask>
{
    protected override void Configure(IObjectTypeDescriptor<UserTask> d)
    {
        d.Name("Task");

        d.BindFieldsExplicitly();

        d.Field(t => t.Id)
            .Type<NonNullType<IdType>>();

        d.Field(t => t.Title)
            .Type<NonNullType<StringType>>();

        d.Field(t => t.Description)
            .Type<StringType>();
    }
}
