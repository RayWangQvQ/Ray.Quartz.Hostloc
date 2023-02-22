#!/usr/bin/env bash
# new Env("hostloc每日投票")
# cron 5 12,23 * * * ray_hostloc_vote.sh

. ray_hostloc_base.sh

cd ./src/Ray.Quartz.Hostloc
# export DOTNET_ENVIRONMENT=Production && \
export Ray_Hostloc_Run=vote && \
dotnet run --ENVIRONMENT=Production