#!/usr/bin/env bash
# new Env("hostloc每日登录")
# cron 30 9 * * * ray_hostloc_login.sh

dir_shell=$QL_DIR/shell
. $dir_shell/share.sh

hostloc_repo="raywangqvq_ray.quartz.hostloc"

echo "repo目录: $dir_repo"
hostloc_repo_dir="$(find $dir_repo -type d -name $hostloc_repo | head -1)"
echo -e "hostloc仓库目录: $hostloc_repo_dir\n"

cd $hostloc_repo_dir
export DOTNET_ENVIRONMENT=Production && \
export Ray_Run=login && \
dotnet run --project ./src/Ray.Quartz.Hostloc