using System.Collections.Generic;

using IAG.Infrastructure.Globalisation.Model;

namespace IAG.Infrastructure.Globalisation.Localizer;

public interface IMessageLocalizer
{
    MessageStructureLocalized Localize(MessageStructure msg);

    List<MessageStructureLocalized> Localize(IEnumerable<MessageStructure> msgs);

    string LocalizeException(System.Exception exception);
}