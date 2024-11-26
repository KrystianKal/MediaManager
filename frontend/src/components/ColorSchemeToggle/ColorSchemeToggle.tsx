'use client';

import {ActionIcon, useComputedColorScheme, useMantineColorScheme} from '@mantine/core';
import {IconMoon, IconSun} from "@tabler/icons-react";
import cx from 'clsx'
import styles from "./ColorShemeToggle.module.css"


export function ColorSchemeToggle() {
  const { setColorScheme } = useMantineColorScheme();
  const computedColorScheme =  useComputedColorScheme('dark', {getInitialValueInEffect: true})

  return (
    <ActionIcon
      onClick={() => setColorScheme(computedColorScheme === 'dark'? 'light' : 'dark')}
      variant={"default"}
      aria-label={"Toggle color scheme"}
    >
      <IconMoon className={cx(styles.icon, styles.light)} stroke={1.5}></IconMoon>
      <IconSun className={cx(styles.icon, styles.dark)} stroke={1.5}></IconSun>
    </ActionIcon>
    // <Group justify="center" mt="xl">
    //   <Button onClick={() => setColorScheme('light')}>Light</Button>
    //   <Button onClick={() => setColorScheme('dark')}>Dark</Button>
    //   <Button onClick={() => setColorScheme('auto')}>Auto</Button>
    // </Group>
  );
}
