import { BookOutlined, CalendarOutlined, SettingOutlined } from '@ant-design/icons';
import { Menu } from 'antd';
import { useState } from 'react';
import { Outlet, useNavigate } from 'react-router-dom';

const items = [
    {
        label: 'Calendar',
        key: 'calendar',
        icon: <CalendarOutlined />,
        selected: true,
    },
    {
        label: 'Subjects',
        key: 'subjects',
        icon: <BookOutlined />,
        disabled: false,
    },
    
    {
        label: (
            <a href="https://ant.design" target="_blank" rel="noopener noreferrer">
                Navigation Four - Link
            </a>
        ),
        key: 'alipay',
    },

    {
        label: 'User',
        key: 'user',
        align: 'right',
        icon: <SettingOutlined />,
        children: [
            {
                label: 'Profile',
                key: 'profile',
            },
        ],
    },
];

function Header() {
    const [current, setCurrent] = useState('mail');
    const navigate = useNavigate();
    const onClick = (e: any) => {
        console.log('click ', e);
        setCurrent(e.key);
        navigate(`/${e.key}`);
    };

    return (
        <>
            <Menu onClick={onClick} selectedKeys={[current]} mode="horizontal" items={items} />
            <Outlet />
        </>
    );
}

export default Header;