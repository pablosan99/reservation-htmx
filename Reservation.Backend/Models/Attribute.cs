namespace Reservation.Backend.Models;

public enum AttributeTypeEnum
{
    STRING = 0,
    NUMBER,
    BOOLEAN,
    CUSTOM_SELECT,
    DATE,
    MULTI_SELECT
}
public class Attribute
{
    public int Id { get; set; }

    public string ObjectName { get; set; } = default!;

    public int ObjectId { get; set; }

    public string ObjectRefName { get; set; } = default!;

    public int? ObjectRefId { get; set; }

    public string AttrName { get; set; } = default!;

    public string AttrValue { get; set; }

    public string AttrDesc { get; set; }

    public int AttrType { get; set; }

    public string DataSource { get; set; }
}