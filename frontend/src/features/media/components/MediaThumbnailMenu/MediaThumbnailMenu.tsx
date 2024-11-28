'use client'
import {Menu} from "@mantine/core";
import React, {useRef} from "react";
import {
  IconArchive,
  IconInfoCircle,
  IconSquareCheck,
  IconSquareX, IconTag,
  IconTagPlus
} from "@tabler/icons-react";
import styles from "./MediaThumbnailMenu.module.css"
import {useMediaGrid} from "@/src/features/media/MediaGridContext";



const IndividualActions = ({ isSelected, mediaId }: { isSelected: boolean, mediaId:string }) => {
  const { toggleSelection } = useMediaGrid();

  return (
    <>
      <Menu.Label>Actions</Menu.Label>
      <Menu.Item
        leftSection={isSelected ? <IconSquareX className={styles.icon}/> : <IconSquareCheck className={styles.icon}/>}
        onClick={() => toggleSelection(mediaId)}
      >
        {isSelected ? 'Deselect' : 'Select'}
      </Menu.Item>
      <Menu.Item leftSection={<IconTagPlus className={styles.icon}/>}>
        Add Tags
      </Menu.Item>
      <Menu.Item leftSection={<IconArchive className={styles.icon}/>}>
        Add To Collection
      </Menu.Item>
    </>
  );
};

const GroupActions = () => {
  const { selectedMedia, clearSelection } = useMediaGrid();

  if (selectedMedia.size < 2) return null;

  return (
    <>
      <Menu.Label>Group Actions</Menu.Label>
      <Menu.Item
        leftSection={<IconSquareX className={styles.icon}/>}
        onClick={clearSelection}
      >
        Deselect All
      </Menu.Item>
      <Menu.Item leftSection={<IconTagPlus className={styles.icon}/>}>
        Add Tags to Selected
      </Menu.Item>
      <Menu.Item leftSection={<IconArchive className={styles.icon}/>}>
        Add Selected to Collection
      </Menu.Item>
    </>
  );
};

type MediaThumbnailMenuProps = { children: React.ReactNode, isSelected: boolean, mediaId: string }
export default function MediaThumbnailMenu({ children, isSelected, mediaId }: MediaThumbnailMenuProps ) 
{
  const [opened, setOpened] = React.useState(false);
  const longPressTimerRef = useRef<NodeJS.Timeout | null>(null);

  const startLongPress = (e: React.TouchEvent) => {
    longPressTimerRef.current = setTimeout(() => {
      e.preventDefault();
      setOpened(true);
    }, 500);
  };

  const cancelLongPress = () => {
    if (longPressTimerRef.current) {
      clearTimeout(longPressTimerRef.current);
      longPressTimerRef.current = null;
    }
  };

  return (
    <Menu
      opened={opened}
      position="right"
      offset={-10}
      arrowPosition="center"
      transitionProps={{transition: 'slide-up', duration: 150}}
      onClose={() => setOpened(false)}
      shadow="md"
      width={240}
    >
      <Menu.Target>
        {React.cloneElement(children as React.ReactElement, {
          onContextMenu: (e: React.MouseEvent) => {
            e.preventDefault();
            e.stopPropagation();
            setOpened(true);
          },
          onTouchStart: startLongPress,
          onTouchEnd: cancelLongPress,
          onTouchCancel: cancelLongPress,
          ...(children as React.ReactElement).props
        })}
      </Menu.Target>
      <Menu.Dropdown>
        <IndividualActions isSelected={isSelected} mediaId={mediaId} />
        <GroupActions />
        <Menu.Divider/>
        <Menu.Item leftSection={<IconInfoCircle className={styles.icon}/>}>
          Info
        </Menu.Item>
      </Menu.Dropdown>
    </Menu>
  );
}
