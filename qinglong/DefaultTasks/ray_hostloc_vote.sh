#!/usr/bin/env bash
# new Env("hostloc每日投票")
# cron 5 12,23 * * * ray_hostloc_vote.sh

dir_shell=$QL_DIR/shell
. $dir_shell/share.sh

hostloc_repo="raywangqvq_ray.quartz.hostloc"

echo "repo目录: $dir_repo"
hostloc_repo_dir="$(find $dir_repo -type d -name $hostloc_repo | head -1)"
echo -e "hostloc仓库目录: $hostloc_repo_dir\n"

cd $hostloc_repo_dir/src/Ray.Quartz.Hostloc
# export DOTNET_ENVIRONMENT=Production && \
export Ray_Hostloc_Run=vote && \
dotnet run --ENVIRONMENT=Production