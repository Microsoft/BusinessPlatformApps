using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.ErrorCode;

namespace Microsoft.Deployment.Actions.AzureCustom.Reddit
{
    [Export(typeof(IAction))]
    public class RandomizeScheduledStartTime : BaseAction
    {
        public const string StartHourParameter = "RangeStartHourUtc";
        public const string EndHourParameter = "RangeEndHourUtc";
        public const string RandomScheduleCronTime = "RandomScheduleCronTime";

        public const int DefaultStartTimeUtc = 1; // 5pm PST to UTC
        public const int DefaultEndTimeUtc = 12; // 7am EST to UTC

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            if (!int.TryParse(request.DataStore.GetValue(StartHourParameter), out int startHourUtc))
            {
                startHourUtc = -1;
            }
            if (!int.TryParse(request.DataStore.GetValue(EndHourParameter), out int endHourUtc))
            {
                endHourUtc = -1;
            }

            if (startHourUtc < 0 && endHourUtc < 0)
            {
                startHourUtc = DefaultStartTimeUtc;
                endHourUtc = DefaultEndTimeUtc;
            }
            else
            {
                var actionResponseInvalidConfiguration = ValidateStartAndEndRange(startHourUtc, endHourUtc);
                if (actionResponseInvalidConfiguration != null)
                {
                    return actionResponseInvalidConfiguration;
                }
            }
            var rng = new Random();
            var randomStartHourUtc = rng.Next(startHourUtc, endHourUtc);
            var randomStartMinute = rng.Next(0, 60);

            // write to ds
            request.DataStore.AddToDataStore(RandomScheduleCronTime, $"0 {randomStartMinute} {randomStartHourUtc} * * *"); // 0 seconds, random minutes, random hours, every day, every month, every day-of-week
            return new ActionResponse(ActionStatus.Success);
        }

        private static ActionResponse ValidateStartAndEndRange(
            int startHourUtc, 
            int endHourUtc
        )
        {
            if (startHourUtc < 0)
            {
                return new ActionResponse(
                    ActionStatus.Failure,
                    null,
                    null,
                    DefaultErrorCodes.DefaultErrorCode,
                    $"{StartHourParameter} was not provided yet {EndHourParameter} was.  Both are required."
                );
            }
            if (startHourUtc > 23)
            {
                return new ActionResponse(
                    ActionStatus.Failure,
                    null,
                    null,
                    DefaultErrorCodes.DefaultErrorCode,
                    $"{StartHourParameter}({startHourUtc}) is larger than 23.  Valid values are between 0 and 23."
                );
            }
            if (endHourUtc < 0)
            {
                return new ActionResponse(
                    ActionStatus.Failure,
                    null,
                    null,
                    DefaultErrorCodes.DefaultErrorCode,
                    $"{EndHourParameter} was not provided yet {StartHourParameter} was.  Both are required."
                );
            }
            if (endHourUtc > 23)
            {
                return new ActionResponse(
                    ActionStatus.Failure,
                    null,
                    null,
                    DefaultErrorCodes.DefaultErrorCode,
                    $"{EndHourParameter}({endHourUtc}) is larger than 23.  Valid values are between 0 and 23."
                );
            }
            if (endHourUtc < startHourUtc)
            {
                return new ActionResponse(
                    ActionStatus.Failure,
                    null,
                    null,
                    DefaultErrorCodes.DefaultErrorCode,
                    $"{StartHourParameter}({startHourUtc}) is larger than {EndHourParameter}({endHourUtc}).  Please provide a range between 0 and 23."
                );
            }
            return null; // all values accounted for, all values within range.
        }
    }
}