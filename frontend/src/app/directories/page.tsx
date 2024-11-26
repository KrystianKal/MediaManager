import {getAllDirectories} from "@/src/features/directories/actions";

export default async function Page(){
  const dirs = await getAllDirectories()
  return <>{dirs.length}</>
}
