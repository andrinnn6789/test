using System;
using System.ComponentModel.DataAnnotations;

using JetBrains.Annotations;

namespace IAG.DataLayer.GenerateCreateScript.Test.TestContext;

[UsedImplicitly]
public class TestEntryA
{
    [Key]
    public Guid Id { get; set; }

    public string TestStringAColumn { get; set; }

    public int TestNumberAColumn { get; set; }

    public bool TestBooleanAColumn { get; set; }
}

[UsedImplicitly]
public class TestEntryB
{
    [Key]
    public Guid Id { get; set; }

    public string TestStringBColumn { get; set; }

    public int TestNumberBColumn { get; set; }

    public bool TestBooleanBColumn { get; set; }
}