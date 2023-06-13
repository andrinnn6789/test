namespace IAG.VinX.CDV.Wamas.Common.DataAccess.DbModel;

public enum LogisticState : short
{
    TransmittedToLogistics = 50,
    ErrorTryAgain = 60,
    LogisticsCompleted = 70,
    LogisticsCompletedWithDifferences = 80
}