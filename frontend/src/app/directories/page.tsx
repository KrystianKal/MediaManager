import {getAllDirectories} from "@/src/features/directories/actions";
import MediaThumbnailMenu from "@/src/features/media/components/MediaThumbnailMenu";
import {Box} from "@mantine/core";

export default async function Page(){
  const dirs = await getAllDirectories()
  return <MediaThumbnailMenu><Box>{dirs.length}</Box></MediaThumbnailMenu>
}
