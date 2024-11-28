import {rem} from "@mantine/core";
import {Spotlight,SpotlightActionData} from "@mantine/spotlight";

import {IconDashboard, IconFileText, IconHome, IconSearch} from "@tabler/icons-react";

const actions: SpotlightActionData[] = [
  {
    id: 'home',
    label: 'Home',
    description: 'Get to home page',
    onClick: () => console.log('Home'),
    leftSection: <IconHome style={{ width: rem(24), height: rem(24) }} stroke={1.5} />,
  },
  {
    id: 'dashboard',
    label: 'Dashboard',
    description: 'Get full information about current system status',
    onClick: () => console.log('Dashboard'),
    leftSection: <IconDashboard style={{ width: rem(24), height: rem(24) }} stroke={1.5} />,
  },
  {
    id: 'documentation',
    label: 'Documentation',
    description: 'Visit documentation to lean more about all features',
    onClick: () => console.log('Documentation'),
    leftSection: <IconFileText style={{ width: rem(24), height: rem(24) }} stroke={1.5} />,
  },
];

export function SearchSpotlight() {
  return (
        <Spotlight
          actions={actions}
          nothingFound="Nothing found..."
          highlightQuery
          limit={7}
          scrollable
          shortcut={"mod + K"}
          searchProps={{
            leftSection: <IconSearch style={{ width: rem(20), height: rem(20) }} stroke={1.5} />,
            placeholder: 'Search...',
          }}
        />
  );
}
